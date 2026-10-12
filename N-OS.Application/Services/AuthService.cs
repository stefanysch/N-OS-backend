using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using N_OS.Application.DTOs;
using N_OS.Application.Exceptions;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class AuthService : IAuthService
{
    /// <summary>Nome do claim com a versão do token (ver Usuario.VersaoToken).</summary>
    public const string ClaimVersaoToken = "ver";

    // duas abas podem renovar ao mesmo tempo com o mesmo refresh token; a que
    // perde a corrida recebe 401 (sem apagar os cookies) se chegou dentro desta janela
    private static readonly TimeSpan JanelaDeConcorrencia = TimeSpan.FromSeconds(10);

    private const string MensagemCredenciaisInvalidas = "E-mail ou senha inválidos.";
    private const string MensagemSessaoExpirada = "Sessão expirada. Faça login novamente.";

    private readonly IUsuarioRepository _repository;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository repository,
        IRefreshTokenRepository refreshTokens,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _repository = repository;
        _refreshTokens = refreshTokens;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UsuarioResponseDTO> Registrar(RegistrarUsuarioDTO input)
    {
        var email = NormalizarEmail(input.Email);

        var usuarioExistente = await _repository.BuscarPorEmail(email);

        if (usuarioExistente != null)
        {
            throw new ArgumentException(
                "Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario
        {
            Nome = input.Nome.Trim(),
            Email = email,
            SenhaHash = SegurancaSenha.Gerar(input.Senha),
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        await _repository.Criar(usuario);
        await _repository.SaveChanges();

        return UsuarioResponseDTO.De(usuario);
    }

    public async Task<bool> CriarPrimeiroUsuario(RegistrarUsuarioDTO input)
    {
        if (await _repository.ExisteAlgum())
            return false;

        Validator.ValidateObject(
            input,
            new ValidationContext(input),
            validateAllProperties: true);

        await Registrar(input);

        return true;
    }

    public async Task<SessaoResultado> Login(
        LoginDTO input,
        string? ip,
        string? userAgent)
    {
        var usuario = await _repository.BuscarPorEmail(input.Email);

        if (usuario == null)
        {
            SegurancaSenha.VerificarFalso(input.Senha);

            throw new CredenciaisInvalidasException(MensagemCredenciaisInvalidas);
        }

        var agora = DateTime.UtcNow;

        if (usuario.EstaBloqueado(agora))
        {
            var minutos = (int)Math.Ceiling((usuario.BloqueadoAte!.Value - agora).TotalMinutes);

            throw new CredenciaisInvalidasException(
                $"Muitas tentativas de login. Tente novamente em {minutos} minuto(s).");
        }

        var senhaCorreta = SegurancaSenha.Verificar(input.Senha, usuario.SenhaHash);

        if (!senhaCorreta || !usuario.Ativo)
        {
            if (!senhaCorreta)
            {
                usuario.RegistrarSenhaIncorreta(agora);

                await _repository.Atualizar(usuario);
                await _repository.SaveChanges();
            }

            throw new CredenciaisInvalidasException(MensagemCredenciaisInvalidas);
        }

        usuario.ZerarTentativasDeLogin();

        // sobe o custo do hash de senhas antigas de forma transparente
        if (SegurancaSenha.PrecisaRehash(usuario.SenhaHash))
        {
            usuario.SenhaHash = SegurancaSenha.Gerar(input.Senha);
        }

        // limpeza: apaga tokens do usuário vencidos há mais de 30 dias
        await _refreshTokens.RemoverExpirados(usuario.Id, agora.AddDays(-30));

        // login novo = família nova de refresh tokens
        var sessao = await EmitirSessao(
            usuario,
            familiaId: Guid.NewGuid(),
            familiaCriadaEm: agora,
            agora,
            ip,
            userAgent);

        await _repository.Atualizar(usuario);
        await _repository.SaveChanges();

        return sessao;
    }

    public async Task<SessaoResultado> Renovar(
        string refreshToken,
        string? ip,
        string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new CredenciaisInvalidasException(MensagemSessaoExpirada);

        var agora = DateTime.UtcNow;

        var atual = await _refreshTokens.BuscarPorHash(CalcularHash(refreshToken));

        if (atual is null || atual.RevogadoEm is not null)
            throw new CredenciaisInvalidasException(MensagemSessaoExpirada);

        if (atual.UsadoEm is { } usadoEm)
        {
            if (agora - usadoEm <= JanelaDeConcorrencia)
                throw new CredenciaisInvalidasException(MensagemSessaoExpirada, encerraSessao: false);

            // um token já trocado voltou a ser apresentado: pode ter sido copiado.
            // por segurança encerra a família inteira.
            _logger.LogWarning(
                "Reuso de refresh token detectado. Usuario {UsuarioId}, família {FamiliaId}, IP {Ip}. Família revogada.",
                atual.UsuarioId, atual.FamiliaId, ip);

            await _refreshTokens.RevogarFamilia(atual.FamiliaId, agora);

            throw new CredenciaisInvalidasException(MensagemSessaoExpirada);
        }

        var limiteDaFamilia = atual.FamiliaCriadaEm.AddDays(LerInt("Sessao:RefreshMaxDias", 30));

        if (atual.ExpiraEm <= agora || limiteDaFamilia <= agora)
        {
            await _refreshTokens.RevogarFamilia(atual.FamiliaId, agora);

            throw new CredenciaisInvalidasException(MensagemSessaoExpirada);
        }

        var usuario = await _repository.BuscarPorId(atual.UsuarioId);

        if (usuario is null || !usuario.Ativo)
        {
            await _refreshTokens.RevogarFamilia(atual.FamiliaId, agora);

            throw new CredenciaisInvalidasException(MensagemSessaoExpirada);
        }

        // uso único: só uma requisição consegue consumir este token
        if (!await _refreshTokens.TentarMarcarComoUsado(atual.Id, agora))
            throw new CredenciaisInvalidasException(MensagemSessaoExpirada, encerraSessao: false);

        var sessao = await EmitirSessao(
            usuario,
            atual.FamiliaId,
            atual.FamiliaCriadaEm,
            agora,
            ip,
            userAgent);

        await _refreshTokens.SaveChanges();

        return sessao;
    }

    public async Task Sair(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;

        var atual = await _refreshTokens.BuscarPorHash(CalcularHash(refreshToken));

        if (atual is null)
            return;

        await _refreshTokens.RevogarFamilia(atual.FamiliaId, DateTime.UtcNow);
    }

    private async Task<SessaoResultado> EmitirSessao(
        Usuario usuario,
        Guid familiaId,
        DateTime familiaCriadaEm,
        DateTime agora,
        string? ip,
        string? userAgent)
    {
        var (tokenAcesso, acessoExpiraEm) = GerarTokenAcesso(usuario, agora);

        // a sessão nunca passa do limite absoluto da família
        var refreshExpiraEm = Min(
            agora.AddDays(LerInt("Sessao:RefreshDias", 7)),
            familiaCriadaEm.AddDays(LerInt("Sessao:RefreshMaxDias", 30)));

        var valorRefresh = GerarValorAleatorio();

        await _refreshTokens.Criar(new RefreshToken
        {
            UsuarioId = usuario.Id,
            TokenHash = CalcularHash(valorRefresh),
            FamiliaId = familiaId,
            FamiliaCriadaEm = familiaCriadaEm,
            CriadoEm = agora,
            ExpiraEm = refreshExpiraEm,
            IpCriacao = Truncar(ip, 45),
            UserAgent = Truncar(userAgent, 300),
        });

        return new SessaoResultado
        {
            TokenAcesso = tokenAcesso,
            TokenAcessoExpiraEm = acessoExpiraEm,
            RefreshToken = valorRefresh,
            RefreshTokenExpiraEm = refreshExpiraEm,
            Usuario = new LoginResponseDTO
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
            },
        };
    }

    internal static string NormalizarEmail(string email) =>
        email.Trim().ToLowerInvariant();

    // 32 bytes aleatórios (256 bits) em base64url
    private static string GerarValorAleatorio() =>
        Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));

    // o valor é aleatório e longo, então SHA-256 puro basta (não precisa de salt)
    internal static string CalcularHash(string valor) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(valor)));

    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

    private static string? Truncar(string? texto, int tamanho) =>
        texto is null || texto.Length <= tamanho ? texto : texto[..tamanho];

    private int LerInt(string chave, int padrao) =>
        int.TryParse(_configuration[chave], out var valor) && valor > 0 ? valor : padrao;

    private (string Token, DateTime ExpiraEm) GerarTokenAcesso(Usuario usuario, DateTime agora)
    {
        var chave = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key não configurada.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var expiraEm = agora.AddMinutes(LerInt("Jwt:ExpiraEmMinutos", 15));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimVersaoToken, usuario.VersaoToken.ToString()),
        };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: agora,
            expires: expiraEm,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUsuarioRepository repository,
        IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<UsuarioResponseDTO> Registrar(RegistrarUsuarioDTO input)
    {
        var usuarioExistente =
            await _repository.BuscarPorEmail(input.Email);

        if (usuarioExistente != null)
        {
            throw new ArgumentException(
                "Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario
        {
            Nome = input.Nome,
            Email = input.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(input.Senha),
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        await _repository.Criar(usuario);
        await _repository.SaveChanges();

        return MapearParaResponse(usuario);
    }

    public async Task<LoginResponseDTO> Login(LoginDTO input)
    {
        var usuario = await _repository.BuscarPorEmail(input.Email);

        if (usuario == null ||
            !usuario.Ativo ||
            !BCrypt.Net.BCrypt.Verify(input.Senha, usuario.SenhaHash))
        {
            throw new ArgumentException("E-mail ou senha inválidos.");
        }

        var (token, expiraEm) = GerarToken(usuario);

        return new LoginResponseDTO
        {
            Token = token,
            ExpiraEm = expiraEm,
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }

    private (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario)
    {
        var chave = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key não configurada.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var minutos = int.TryParse(
            _configuration["Jwt:ExpiraEmMinutos"], out var valor)
                ? valor
                : 480;

        var expiraEm = DateTime.UtcNow.AddMinutes(minutos);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
        };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }

    private static UsuarioResponseDTO MapearParaResponse(Usuario usuario)
    {
        return new UsuarioResponseDTO
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            CriadoEm = usuario.CriadoEm,
            Ativo = usuario.Ativo
        };
    }
}

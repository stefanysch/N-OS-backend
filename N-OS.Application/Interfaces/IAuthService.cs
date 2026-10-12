using N_OS.Application.DTOs;

namespace N_OS.Application.Interfaces;

public interface IAuthService
{
    Task<UsuarioResponseDTO> Registrar(RegistrarUsuarioDTO input);

    Task<SessaoResultado> Login(LoginDTO input, string? ip, string? userAgent);

    /// <summary>
    /// Troca um refresh token válido por um novo par de tokens. O refresh token
    /// antigo deixa de valer. Lança CredenciaisInvalidasException se a sessão
    /// não puder ser renovada.
    /// </summary>
    Task<SessaoResultado> Renovar(string refreshToken, string? ip, string? userAgent);

    /// <summary>Encerra a sessão (toda a família do refresh token). Não falha se o token for inválido.</summary>
    Task Sair(string? refreshToken);

    /// <summary>
    /// Cria o primeiro usuário quando o banco ainda não tem nenhum.
    /// Devolve false se já existe usuário (não faz nada).
    /// </summary>
    Task<bool> CriarPrimeiroUsuario(RegistrarUsuarioDTO input);
}

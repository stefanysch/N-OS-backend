namespace N_OS.Application.DTOs;

/// <summary>
/// Dados do usuário logado. Os tokens NÃO vão no corpo: seguem em cookies
/// HttpOnly, fora do alcance de JavaScript.
/// </summary>
public class LoginResponseDTO
{
    public int UsuarioId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}

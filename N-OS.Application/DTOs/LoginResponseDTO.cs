namespace N_OS.Application.DTOs;

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }

    public int UsuarioId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}

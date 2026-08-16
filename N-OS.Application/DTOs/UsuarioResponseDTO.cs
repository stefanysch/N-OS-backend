namespace N_OS.Application.DTOs;

public class UsuarioResponseDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }
}

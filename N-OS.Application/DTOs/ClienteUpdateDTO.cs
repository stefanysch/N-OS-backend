namespace N_OS.Application.DTOs;

public class ClienteUpdateDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Email { get; set; }
}
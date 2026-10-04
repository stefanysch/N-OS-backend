namespace N_OS.Application.DTOs;

public class ClienteListagemDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Documento { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Cidade { get; set; }

    public string? Estado { get; set; }

    public int QuantidadeVeiculos { get; set; }

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }
}

namespace N_OS.Application.DTOs;

public class ClienteResponseDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Documento { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Cep { get; set; }

    public string? Logradouro { get; set; }

    public string? Numero { get; set; }

    public string? Complemento { get; set; }

    public string? Bairro { get; set; }

    public string? Cidade { get; set; }

    public string? Estado { get; set; }

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }

    public ICollection<VeiculoResponseDTO> Veiculos { get; set; } = [];
}
namespace N_OS.Application.DTOs;

public class ClienteResponseDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Documento { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string Cep { get; set; } = string.Empty;

    public string Logradouro { get; set; } = string.Empty;

    public string Numero { get; set; } = string.Empty;

    public string? Complemento { get; set; }

    public string Bairro { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }

    public ICollection<VeiculoResponseDTO> Veiculos { get; set; } = [];
}
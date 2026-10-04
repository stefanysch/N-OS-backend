namespace N_OS.Application.DTOs;

public class VeiculoListagemDTO
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string ClienteNome { get; set; } = string.Empty;

    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public int? Ano { get; set; }

    public string? Cor { get; set; }

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }
}

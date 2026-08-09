namespace N_OS.Application.DTOs;

public class VeiculoResponseDTO
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public int Ano { get; set; }

    public string Cor { get; set; } = string.Empty;

    public string? Chassi { get; set; }

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }
}
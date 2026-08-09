namespace N_OS.Domain.Entities;

public class Veiculo
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public int Ano { get; set; }

    public string Cor { get; set; } = string.Empty;

    public string? Chassi { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public bool Ativo { get; set; } = true;
}
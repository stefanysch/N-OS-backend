namespace N_OS.Domain.Entities;

public class Peca
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public int Quantidade { get; set; }

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }
}
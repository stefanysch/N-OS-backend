namespace N_OS.Application.DTOs;

public class PecaListagemDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }
}

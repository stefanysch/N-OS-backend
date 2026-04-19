namespace N_OS.Application.DTOs;

public class PecaCreateDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Quantidade { get; set; }
}
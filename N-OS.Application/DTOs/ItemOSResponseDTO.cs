namespace N_OS.Application.DTOs;

public class ItemOSResponseDTO
{
    public int Id { get; set; }

    public int Quantidade { get; set; }

    public decimal ValorAplicado { get; set; }
    public decimal Subtotal { get; set; }
    public int? PecaId { get; set; }
    public string? PecaNome { get; set; }
    public int? ServicoId { get; set; }
    public string? ServicoNome { get; set; }
}
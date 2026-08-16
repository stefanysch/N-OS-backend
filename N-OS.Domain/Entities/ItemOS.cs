using System.Text.Json.Serialization;

namespace N_OS.Domain.Entities;

public class ItemOS
{
    public int Id { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorAplicado { get; set; }
    public decimal Subtotal { get; set; }
    public int? PecaId { get; set; }
    public Peca? Peca { get; set; }
    public int? ServicoId { get; set; }
    public Servico? Servico { get; set; }
    public int OrdemDeServicoId { get; set; }
    [JsonIgnore]
    public OrdemDeServico OrdemDeServico { get; set; } = null!;
}
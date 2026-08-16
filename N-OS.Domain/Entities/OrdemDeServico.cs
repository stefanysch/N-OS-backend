using N_OS.Domain.Enums;

namespace N_OS.Domain.Entities
{
    public class OrdemDeServico
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public Veiculo Veiculo { get; set; } = null!;
        public StatusOS Status { get; set; } = StatusOS.Aguardando;
        public string DescricaoProblema { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public decimal Desconto { get; set; }
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;
        public ICollection<ItemOS> ItensOS { get; set; } = [];
    }
}
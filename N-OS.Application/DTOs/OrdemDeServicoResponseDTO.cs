namespace N_OS.Application.DTOs;

public class OrdemDeServicoResponseDTO
{
    public int Id { get; set; }

    public int VeiculoId { get; set; }

    public int Status { get; set; }

    public string DescricaoProblema { get; set; } = string.Empty;

    public string Observacoes { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public decimal Desconto { get; set; }

    public DateTime DataAbertura { get; set; }

    public bool Ativo { get; set; }

    public List<ItemOSResponseDTO> Itens { get; set; } = new();
}
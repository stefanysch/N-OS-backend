namespace N_OS.Application.DTOs;

public class OrdemDeServicoListagemDTO
{
    public int Id { get; set; }

    public string ClienteNome { get; set; } = string.Empty;

    public string Placa { get; set; } = string.Empty;

    public int Status { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime DataAbertura { get; set; }

    public bool Ativo { get; set; }
}

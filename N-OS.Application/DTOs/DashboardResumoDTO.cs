namespace N_OS.Application.DTOs;

public class DashboardResumoDTO
{
    public int VeiculosAtivos { get; set; }

    public int ClientesNovosUltimos30Dias { get; set; }

    public int OrdensAtivasTotal { get; set; }

    public List<StatusContagemDTO> OrdensPorStatus { get; set; } = [];

    public int OrdensConcluidasUltimos30Dias { get; set; }

    public List<ClienteRecenteDTO> ClientesRecentes { get; set; } = [];

    public List<OrdemResumoDTO> OrdensRecemFechadas { get; set; } = [];

    public List<OrdemResumoDTO> OrdensAFazer { get; set; } = [];
}

public record StatusContagemDTO(int Status, int Quantidade);

public record ClienteRecenteDTO(int Id, string Nome, DateTime CriadoEm);

public record OrdemResumoDTO(
    int Id,
    string ClienteNome,
    string VeiculoPlaca,
    int Status,
    decimal ValorTotal,
    DateTime DataAbertura,
    DateTime? DataConclusao);

using N_OS.Application.DTOs;

namespace N_OS.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResumoDTO> ObterResumo();

    Task<FaturamentoResumoDTO> ObterFaturamento(DateOnly? de, DateOnly? ate);
}

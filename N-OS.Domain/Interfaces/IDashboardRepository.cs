using N_OS.Domain.Entities;
using N_OS.Domain.Enums;

namespace N_OS.Domain.Interfaces;

public interface IDashboardRepository
{
    Task<int> ContarVeiculosAtivos();

    Task<int> ContarClientesNovosDesde(DateTime desde);

    Task<IEnumerable<Cliente>> ListarClientesRecentes(int quantidade);

    Task<List<(StatusOS Status, int Quantidade)>> ContarOrdensAtivasPorStatus();

    Task<int> ContarOrdensConcluidasDesde(DateTime desde);

    Task<IEnumerable<OrdemDeServico>> ListarRecemConcluidas(int quantidade);

    Task<IEnumerable<OrdemDeServico>> ListarOrdensAFazer(int quantidade);

    Task<List<(DateTime Dia, decimal Valor)>> ListarFaturamentoPorDiaEntre(DateTime inicio, DateTime fim);
}

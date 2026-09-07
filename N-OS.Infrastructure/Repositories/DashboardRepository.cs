using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Enums;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> ContarVeiculosAtivos()
    {
        return _context.Veiculos
            .CountAsync(v => v.Ativo);
    }

    public Task<int> ContarClientesNovosDesde(DateTime desde)
    {
        return _context.Clientes
            .CountAsync(c => c.Ativo && c.CriadoEm >= desde);
    }

    public async Task<IEnumerable<Cliente>> ListarClientesRecentes(int quantidade)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Ativo)
            .OrderByDescending(c => c.CriadoEm)
            .Take(quantidade)
            .ToListAsync();
    }

    public async Task<List<(StatusOS Status, int Quantidade)>> ContarOrdensAtivasPorStatus()
    {
        var grupos = await _context.OrdensDeServico
            .Where(os => os.Ativo && os.Status != StatusOS.Concluida)
            .GroupBy(os => os.Status)
            .Select(g => new { Status = g.Key, Quantidade = g.Count() })
            .ToListAsync();

        return grupos
            .Select(g => (g.Status, g.Quantidade))
            .ToList();
    }

    public Task<int> ContarOrdensConcluidasDesde(DateTime desde)
    {
        return _context.OrdensDeServico
            .CountAsync(os =>
                os.Ativo &&
                os.Status == StatusOS.Concluida &&
                os.DataConclusao != null &&
                os.DataConclusao >= desde);
    }

    public async Task<IEnumerable<OrdemDeServico>> ListarRecemConcluidas(int quantidade)
    {
        return await _context.OrdensDeServico
            .AsNoTracking()
            .Include(os => os.Veiculo)
                .ThenInclude(v => v.Cliente)
            .Where(os =>
                os.Ativo &&
                os.Status == StatusOS.Concluida &&
                os.DataConclusao != null)
            .OrderByDescending(os => os.DataConclusao)
            .Take(quantidade)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrdemDeServico>> ListarOrdensAFazer(int quantidade)
    {
        return await _context.OrdensDeServico
            .AsNoTracking()
            .Include(os => os.Veiculo)
                .ThenInclude(v => v.Cliente)
            .Where(os =>
                os.Ativo &&
                os.Status != StatusOS.Concluida)
            .OrderBy(os => os.DataAbertura)
            .Take(quantidade)
            .ToListAsync();
    }

    public async Task<List<(DateTime Dia, decimal Valor)>> ListarFaturamentoPorDiaEntre(
        DateTime inicio,
        DateTime fim)
    {
        var grupos = await _context.OrdensDeServico
            .Where(os =>
                os.Ativo &&
                os.Status == StatusOS.Concluida &&
                os.DataConclusao != null &&
                os.DataConclusao >= inicio &&
                os.DataConclusao <= fim)
            .GroupBy(os => os.DataConclusao!.Value.Date)
            .Select(g => new { Dia = g.Key, Valor = g.Sum(os => os.ValorTotal) })
            .ToListAsync();

        return grupos
            .Select(g => (g.Dia, g.Valor))
            .ToList();
    }
}

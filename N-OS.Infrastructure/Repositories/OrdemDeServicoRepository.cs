using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Repositories;

public class OrdemDeServicoRepository : IOrdemDeServicoRepository
{
    private readonly AppDbContext _context;

    public OrdemDeServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrdemDeServico>> Listar()
    {
        return await _context.OrdensDeServico
            .Include(os => os.Veiculo)
                .ThenInclude(v => v.Cliente)
            .Include(os => os.ItensOS)
                .ThenInclude(item => item.Peca)
            .Include(os => os.ItensOS)
                .ThenInclude(item => item.Servico)
            .ToListAsync();
    }

    public async Task<OrdemDeServico?> BuscarPorId(int id)
    {
        return await _context.OrdensDeServico
            .Include(os => os.Veiculo)
                .ThenInclude(v => v.Cliente)
            .Include(os => os.ItensOS)
                .ThenInclude(item => item.Peca)
            .Include(os => os.ItensOS)
                .ThenInclude(item => item.Servico)
            .FirstOrDefaultAsync(os => os.Id == id);
    }

    public Task Criar(OrdemDeServico ordemDeServico)
    {
        _context.OrdensDeServico.Add(ordemDeServico);

        return Task.CompletedTask;
    }

    public Task Atualizar(OrdemDeServico ordemDeServico)
    {
        _context.OrdensDeServico.Update(ordemDeServico);

        return Task.CompletedTask;
    }

    public async Task<bool> PossuiOrdemDeServicoAtiva(int veiculoId)
    {
        return await _context.OrdensDeServico
            .AnyAsync(os =>
                os.VeiculoId == veiculoId &&
                os.Ativo);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
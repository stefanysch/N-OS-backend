using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Repositories;

public class ServicoRepository : IServicoRepository
{
    private readonly AppDbContext _context;

    public ServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Servico>> Listar()
    {
        return await _context.Servicos.ToListAsync();
    }

    public async Task<Servico?> BuscarPorId(int id)
    {
        return await _context.Servicos.FindAsync(id);
    }

    public async Task<Servico> Criar(Servico servico)
    {
        _context.Servicos.Add(servico);

        return servico;
    }

    public Task<Servico?> Atualizar(Servico servico)
    {
        _context.Servicos.Update(servico);

        return Task.FromResult<Servico?>(servico);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
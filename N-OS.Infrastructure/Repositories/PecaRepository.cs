using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Repositories;

public class PecaRepository : IPecaRepository
{
    private readonly AppDbContext _context;

    public PecaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Peca>> Listar()
    {
        return await _context.Pecas.ToListAsync();
    }

    public async Task<Peca?> BuscarPorId(int id)
    {
        return await _context.Pecas.FindAsync(id);
    }

    public async Task<Peca> Criar(Peca peca)
    {
        _context.Pecas.Add(peca);

        return peca;
    }

    public Task<Peca?> Atualizar(Peca peca)
    {
        _context.Pecas.Update(peca);

        return Task.FromResult<Peca?>(peca);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
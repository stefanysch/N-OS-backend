using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> Listar()
    {
        return await _context.Clientes
            .Include(c => c.Veiculos)
            .ToListAsync();
    }

    public async Task<Cliente?> BuscarPorId(int id)
    {
        return await _context.Clientes
            .Include(c => c.Veiculos)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task Criar(Cliente cliente)
    {
        _context.Clientes.Add(cliente);

        return Task.CompletedTask;
    }

    public Task Atualizar(Cliente cliente)
    {
        _context.Clientes.Update(cliente);

        return Task.CompletedTask;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
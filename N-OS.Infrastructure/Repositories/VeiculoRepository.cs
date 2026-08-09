using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Repositories;

public class VeiculoRepository : IVeiculoRepository
{
    private readonly AppDbContext _context;

    public VeiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Veiculo>> Listar()
    {
        return await _context.Veiculos
            .Include(v => v.Cliente)
            .ToListAsync();
    }

    public async Task<IEnumerable<Veiculo>> ListarPorCliente(int clienteId)
    {
        return await _context.Veiculos
            .Where(v => v.ClienteId == clienteId)
            .ToListAsync();
    }

    public async Task<Veiculo?> BuscarPorId(int id)
    {
        return await _context.Veiculos
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public Task Criar(Veiculo veiculo)
    {
        _context.Veiculos.Add(veiculo);

        return Task.CompletedTask;
    }

    public Task Atualizar(Veiculo veiculo)
    {
        _context.Veiculos.Update(veiculo);

        return Task.CompletedTask;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
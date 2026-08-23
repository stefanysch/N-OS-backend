using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Enums;
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

    public async Task<Cliente?> BuscarPorDocumento(string numero)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Documento.Numero == numero);
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

    public async Task<IEnumerable<Veiculo>> ListarVeiculos(int clienteId)
    {
        return await _context.Veiculos
            .Where(v => v.ClienteId == clienteId)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> PlacasDeVeiculosComOSAtiva(int clienteId)
    {
        return await _context.OrdensDeServico
        .Where(os => 
            os.Veiculo.ClienteId == clienteId &&
            os.Status != StatusOS.Concluida &&
            os.Ativo)
        .Select(os => os.Veiculo.Placa)
        .Distinct()
        .ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
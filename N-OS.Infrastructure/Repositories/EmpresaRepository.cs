using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    private readonly AppDbContext _context;

    public EmpresaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Empresa?> Obter()
    {
        return await _context.Empresas.FirstOrDefaultAsync();
    }

    public Task Criar(Empresa empresa)
    {
        _context.Empresas.Add(empresa);

        return Task.CompletedTask;
    }

    public Task Atualizar(Empresa empresa)
    {
        _context.Empresas.Update(empresa);

        return Task.CompletedTask;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}

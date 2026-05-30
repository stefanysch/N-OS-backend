using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Services;

public class PecaService
{
    private readonly AppDbContext _context;

    public PecaService(AppDbContext context)
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

    public async Task<Peca> Criar(PecaCreateDTO input)
    {
        var peca = new Peca
        {
            Nome = input.Nome,
            Descricao = input.Descricao,
            Valor = input.Valor,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        _context.Pecas.Add(peca);

        await _context.SaveChangesAsync();

        return peca;
    }

    public async Task<Peca?> Atualizar(
        int id,
        PecaUpdateDTO input)
    {
        var peca =
            await _context.Pecas.FindAsync(id);

        if (peca == null)
            return null;

        peca.Nome = input.Nome;
        peca.Descricao = input.Descricao;
        peca.Valor = input.Valor;

        await _context.SaveChangesAsync();

        return peca;
    }

    public async Task<bool> Inativar(int id)
    {
        var peca =
            await _context.Pecas.FindAsync(id);

        if (peca == null)
            return false;

        peca.Ativo = false;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Reativar(int id)
    {
        var peca =
            await _context.Pecas.FindAsync(id);

        if (peca == null)
            return false;

        peca.Ativo = true;

        await _context.SaveChangesAsync();

        return true;
    }
}
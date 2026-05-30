using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Services;

public class ServicoService
{
    private readonly AppDbContext _context;

    public ServicoService(AppDbContext context)
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

    public async Task<Servico> Criar(
        ServicoCreateDTO input)
    {
        var servico = new Servico
        {
            Nome = input.Nome,
            Descricao = input.Descricao,
            Valor = input.Valor,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        _context.Servicos.Add(servico);

        await _context.SaveChangesAsync();

        return servico;
    }

    public async Task<Servico?> Atualizar(
        int id,
        ServicoUpdateDTO input)
    {
        var servico =
            await _context.Servicos.FindAsync(id);

        if (servico == null)
            return null;

        servico.Nome = input.Nome;
        servico.Descricao = input.Descricao;
        servico.Valor = input.Valor;

        await _context.SaveChangesAsync();

        return servico;
    }

    public async Task<bool> Inativar(int id)
    {
        var servico =
            await _context.Servicos.FindAsync(id);

        if (servico == null)
            return false;

        servico.Ativo = false;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Reativar(int id)
    {
        var servico =
            await _context.Servicos.FindAsync(id);

        if (servico == null)
            return false;

        servico.Ativo = true;

        await _context.SaveChangesAsync();

        return true;
    }
}
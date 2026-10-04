using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Application.Pagination;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;
using static N_OS.Infrastructure.Queries.ConsultaHelper;

namespace N_OS.Infrastructure.Queries;

public class ServicoQueries : IServicoQueries
{
    private readonly AppDbContext _context;

    public ServicoQueries(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ServicoListagemDTO>> Listar(PageQuery query)
    {
        var servicos = _context.Servicos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var padrao = PadraoLike(query.Q);

            servicos = servicos.Where(s =>
                EF.Functions.ILike(s.Nome, padrao) ||
                EF.Functions.ILike(s.Descricao, padrao));
        }

        var total = await servicos.CountAsync();

        var items = await Ordenar(servicos, query)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new ServicoListagemDTO
            {
                Id = s.Id,
                Nome = s.Nome,
                Descricao = s.Descricao,
                Valor = s.Valor,
                CriadoEm = s.CriadoEm,
                Ativo = s.Ativo
            })
            .ToListAsync();

        return new PagedResult<ServicoListagemDTO>(
            items, query.Page, query.PageSize, total);
    }

    // ativos primeiro; depois o campo pedido (whitelist); Id como desempate.
    private static IQueryable<Servico> Ordenar(
        IQueryable<Servico> servicos, PageQuery query)
    {
        var ordenados = servicos.OrderByDescending(s => s.Ativo);

        var desc = query.Descendente;

        ordenados = query.Sort?.Trim().ToLowerInvariant() switch
        {
            "nome" => Then(ordenados, s => s.Nome, desc),
            "valor" => Then(ordenados, s => s.Valor, desc),
            "criadoem" => Then(ordenados, s => s.CriadoEm, desc),
            _ => Then(ordenados, s => s.Nome, false)
        };

        return ordenados.ThenBy(s => s.Id);
    }
}

using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Application.Pagination;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;
using static N_OS.Infrastructure.Queries.ConsultaHelper;

namespace N_OS.Infrastructure.Queries;

public class PecaQueries : IPecaQueries
{
    private readonly AppDbContext _context;

    public PecaQueries(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PecaListagemDTO>> Listar(PageQuery query)
    {
        var pecas = _context.Pecas.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var padrao = PadraoLike(query.Q);

            pecas = pecas.Where(p =>
                EF.Functions.ILike(p.Nome, padrao) ||
                EF.Functions.ILike(p.Descricao, padrao));
        }

        var total = await pecas.CountAsync();

        var items = await Ordenar(pecas, query)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PecaListagemDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Valor = p.Valor,
                CriadoEm = p.CriadoEm,
                Ativo = p.Ativo
            })
            .ToListAsync();

        return new PagedResult<PecaListagemDTO>(
            items, query.Page, query.PageSize, total);
    }

    // ativas primeiro; depois o campo pedido (whitelist); Id como desempate.
    private static IQueryable<Peca> Ordenar(
        IQueryable<Peca> pecas, PageQuery query)
    {
        var ordenadas = pecas.OrderByDescending(p => p.Ativo);

        var desc = query.Descendente;

        ordenadas = query.Sort?.Trim().ToLowerInvariant() switch
        {
            "nome" => Then(ordenadas, p => p.Nome, desc),
            "valor" => Then(ordenadas, p => p.Valor, desc),
            "criadoem" => Then(ordenadas, p => p.CriadoEm, desc),
            _ => Then(ordenadas, p => p.Nome, false)
        };

        return ordenadas.ThenBy(p => p.Id);
    }
}

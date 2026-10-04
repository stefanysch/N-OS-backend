using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Application.Pagination;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;
using static N_OS.Infrastructure.Queries.ConsultaHelper;

namespace N_OS.Infrastructure.Queries;

public class ClienteQueries : IClienteQueries
{
    private readonly AppDbContext _context;

    public ClienteQueries(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ClienteListagemDTO>> Listar(PageQuery query)
    {
        var clientes = _context.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var padrao = PadraoLike(query.Q);

            clientes = clientes.Where(c =>
                EF.Functions.ILike(c.Nome, padrao) ||
                EF.Functions.ILike(c.Documento.Numero, padrao) ||
                EF.Functions.ILike(c.Telefone, padrao) ||
                EF.Functions.ILike(c.Email, padrao));
        }

        var total = await clientes.CountAsync();

        var items = await Ordenar(clientes, query)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new ClienteListagemDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Documento = c.Documento.Numero,
                Telefone = c.Telefone,
                Email = c.Email,
                Cidade = c.Endereco != null ? c.Endereco.Cidade : null,
                Estado = c.Endereco != null ? c.Endereco.Estado : null,
                QuantidadeVeiculos = c.Veiculos.Count(),
                CriadoEm = c.CriadoEm,
                Ativo = c.Ativo
            })
            .ToListAsync();

        return new PagedResult<ClienteListagemDTO>(
            items, query.Page, query.PageSize, total);
    }

    // ativos primeiro; depois o campo pedido (whitelist); Id como desempate.
    private static IQueryable<Cliente> Ordenar(
        IQueryable<Cliente> clientes, PageQuery query)
    {
        var ordenados = clientes.OrderByDescending(c => c.Ativo);

        var desc = query.Descendente;

        ordenados = query.Sort?.Trim().ToLowerInvariant() switch
        {
            "nome" => Then(ordenados, c => c.Nome, desc),
            "criadoem" => Then(ordenados, c => c.CriadoEm, desc),
            _ => Then(ordenados, c => c.Nome, false)
        };

        return ordenados.ThenBy(c => c.Id);
    }
}

using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Application.Pagination;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;
using static N_OS.Infrastructure.Queries.ConsultaHelper;

namespace N_OS.Infrastructure.Queries;

public class VeiculoQueries : IVeiculoQueries
{
    private readonly AppDbContext _context;

    public VeiculoQueries(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<VeiculoListagemDTO>> Listar(PageQuery query)
    {
        var veiculos = _context.Veiculos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var padrao = PadraoLike(query.Q);

            veiculos = veiculos.Where(v =>
                EF.Functions.ILike(v.Placa, padrao) ||
                EF.Functions.ILike(v.Marca, padrao) ||
                EF.Functions.ILike(v.Modelo, padrao) ||
                EF.Functions.ILike(v.Cliente.Nome, padrao));
        }

        var total = await veiculos.CountAsync();

        var items = await Ordenar(veiculos, query)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(v => new VeiculoListagemDTO
            {
                Id = v.Id,
                ClienteId = v.ClienteId,
                ClienteNome = v.Cliente.Nome,
                Placa = v.Placa,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Ano = v.Ano,
                Cor = v.Cor,
                CriadoEm = v.CriadoEm,
                Ativo = v.Ativo
            })
            .ToListAsync();

        return new PagedResult<VeiculoListagemDTO>(
            items, query.Page, query.PageSize, total);
    }

    // ativos primeiro; depois o campo pedido (whitelist); Id como desempate.
    private static IQueryable<Veiculo> Ordenar(
        IQueryable<Veiculo> veiculos, PageQuery query)
    {
        var ordenados = veiculos.OrderByDescending(v => v.Ativo);

        var desc = query.Descendente;

        ordenados = query.Sort?.Trim().ToLowerInvariant() switch
        {
            "placa" => Then(ordenados, v => v.Placa, desc),
            "marca" => Then(ordenados, v => v.Marca, desc),
            "modelo" => Then(ordenados, v => v.Modelo, desc),
            "ano" => Then(ordenados, v => v.Ano, desc),
            "cliente" => Then(ordenados, v => v.Cliente.Nome, desc),
            "criadoem" => Then(ordenados, v => v.CriadoEm, desc),
            _ => Then(ordenados, v => v.Placa, false)
        };

        return ordenados.ThenBy(v => v.Id);
    }
}

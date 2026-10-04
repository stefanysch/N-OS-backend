using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Application.Pagination;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;
using static N_OS.Infrastructure.Queries.ConsultaHelper;

namespace N_OS.Infrastructure.Queries;

public class OrdemDeServicoQueries : IOrdemDeServicoQueries
{
    private readonly AppDbContext _context;

    public OrdemDeServicoQueries(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<OrdemDeServicoListagemDTO>> Listar(PageQuery query)
    {
        var ordens = _context.OrdensDeServico.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var padrao = ConsultaHelper.PadraoLike(query.Q);

            ordens = ordens.Where(os =>
                EF.Functions.ILike(os.Veiculo.Cliente.Nome, padrao) ||
                EF.Functions.ILike(os.Veiculo.Placa, padrao));
        }

        var total = await ordens.CountAsync();

        var items = await Ordenar(ordens, query)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(os => new OrdemDeServicoListagemDTO
            {
                Id = os.Id,
                ClienteNome = os.Veiculo.Cliente.Nome,
                Placa = os.Veiculo.Placa,
                Status = (int)os.Status,
                ValorTotal = os.ValorTotal,
                DataAbertura = os.DataAbertura,
                Ativo = os.Ativo
            })
            .ToListAsync();

        return new PagedResult<OrdemDeServicoListagemDTO>(
            items, query.Page, query.PageSize, total);
    }

    // ativas primeiro, inativas por último; depois o campo pedido (whitelist)
    // e o Id como desempate para a ordem ser estável entre páginas.
    private static IQueryable<OrdemDeServico> Ordenar(
        IQueryable<OrdemDeServico> ordens, PageQuery query)
    {
        var ordenada = ordens.OrderByDescending(os => os.Ativo);

        var campo = query.Sort?.Trim().ToLowerInvariant();
        var desc = query.Descendente;

        ordenada = campo switch
        {
            "cliente" => Then(ordenada, os => os.Veiculo.Cliente.Nome, desc),
            "placa" => Then(ordenada, os => os.Veiculo.Placa, desc),
            "status" => Then(ordenada, os => os.Status, desc),
            "valortotal" => Then(ordenada, os => os.ValorTotal, desc),
            "dataabertura" => Then(ordenada, os => os.DataAbertura, desc),
            _ => Then(ordenada, os => os.DataAbertura, true)
        };

        return ordenada.ThenBy(os => os.Id);
    }

}

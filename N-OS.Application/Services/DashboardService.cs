using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class DashboardService : IDashboardService
{
    private const int JanelaEmDias = 30;
    private const int QuantidadeNasListas = 6;
    private const int JanelaPadraoFaturamentoDias = 30;
    private const int LimiteDiasFaturamento = 366;

    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardResumoDTO> ObterResumo()
    {
        var desde = DateTime.UtcNow.AddDays(-JanelaEmDias);

        var veiculosAtivos = await _repository.ContarVeiculosAtivos();

        var clientesNovos = await _repository.ContarClientesNovosDesde(desde);

        var clientesRecentes = await _repository.ListarClientesRecentes(QuantidadeNasListas);

        var ordensPorStatus = await _repository.ContarOrdensAtivasPorStatus();

        var ordensConcluidas = await _repository.ContarOrdensConcluidasDesde(desde);

        var recemFechadas = await _repository.ListarRecemConcluidas(QuantidadeNasListas);

        var aFazer = await _repository.ListarOrdensAFazer(QuantidadeNasListas);

        return new DashboardResumoDTO
        {
            VeiculosAtivos = veiculosAtivos,

            ClientesNovosUltimos30Dias = clientesNovos,

            OrdensAtivasTotal = ordensPorStatus.Sum(p => p.Quantidade),

            OrdensPorStatus = ordensPorStatus
                .Select(p => new StatusContagemDTO((int)p.Status, p.Quantidade))
                .ToList(),

            OrdensConcluidasUltimos30Dias = ordensConcluidas,

            ClientesRecentes = clientesRecentes
                .Select(c => new ClienteRecenteDTO(c.Id, c.Nome, c.CriadoEm))
                .ToList(),

            OrdensRecemFechadas = recemFechadas
                .Select(MapearOrdemResumo)
                .ToList(),

            OrdensAFazer = aFazer
                .Select(MapearOrdemResumo)
                .ToList(),
        };
    }

    public async Task<FaturamentoResumoDTO> ObterFaturamento(DateOnly? de, DateOnly? ate)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var fim = ate ?? hoje;
        var inicio = de ?? fim.AddDays(-(JanelaPadraoFaturamentoDias - 1));

        if (inicio > fim)
            (inicio, fim) = (fim, inicio);

        if (fim.DayNumber - inicio.DayNumber > LimiteDiasFaturamento)
            inicio = fim.AddDays(-LimiteDiasFaturamento);

        var inicioUtc = DateTime.SpecifyKind(
            inicio.ToDateTime(TimeOnly.MinValue),
            DateTimeKind.Utc);

        var fimUtc = DateTime.SpecifyKind(
            fim.ToDateTime(TimeOnly.MaxValue),
            DateTimeKind.Utc);

        var dados = await _repository.ListarFaturamentoPorDiaEntre(inicioUtc, fimUtc);

        var porDia = PreencherDiasFaltantes(dados, inicio, fim);

        return new FaturamentoResumoDTO(
            inicio,
            fim,
            porDia.Sum(d => d.Valor),
            porDia);
    }

    private static List<FaturamentoDiaDTO> PreencherDiasFaltantes(
        List<(DateTime Dia, decimal Valor)> dados,
        DateOnly inicio,
        DateOnly fim)
    {
        var valoresPorDia = dados.ToDictionary(
            d => DateOnly.FromDateTime(d.Dia),
            d => d.Valor);

        var resultado = new List<FaturamentoDiaDTO>();

        for (var dia = inicio; dia <= fim; dia = dia.AddDays(1))
        {
            resultado.Add(new FaturamentoDiaDTO(
                dia,
                valoresPorDia.GetValueOrDefault(dia, 0)));
        }

        return resultado;
    }

    private static OrdemResumoDTO MapearOrdemResumo(OrdemDeServico ordemDeServico)
    {
        return new OrdemResumoDTO(
            ordemDeServico.Id,
            ordemDeServico.Veiculo.Cliente.Nome,
            ordemDeServico.Veiculo.Placa,
            (int)ordemDeServico.Status,
            ordemDeServico.ValorTotal,
            ordemDeServico.DataAbertura,
            ordemDeServico.DataConclusao);
    }
}

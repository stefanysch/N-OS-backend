namespace N_OS.Application.DTOs;

public record FaturamentoResumoDTO(
    DateOnly PeriodoInicio,
    DateOnly PeriodoFim,
    decimal Total,
    List<FaturamentoDiaDTO> PorDia);

public record FaturamentoDiaDTO(DateOnly Dia, decimal Valor);

using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

/// <summary>Indicadores para a tela inicial.</summary>
[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>Resumo geral do sistema.</summary>
    /// <remarks>
    /// Veículos ativos, clientes novos e ordens concluídas nos últimos 30 dias,
    /// ordens ativas por status, clientes recentes e listas de ordens
    /// recém-fechadas e a fazer.
    /// </remarks>
    /// <response code="200">Resumo do dashboard.</response>
    [HttpGet("resumo")]
    [ProducesResponseType(typeof(DashboardResumoDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterResumo()
    {
        var resumo = await _dashboardService.ObterResumo();

        return Ok(resumo);
    }

    /// <summary>Faturamento por dia em um período.</summary>
    /// <param name="de">Início do período (`yyyy-MM-dd`). Opcional.</param>
    /// <param name="ate">Fim do período (`yyyy-MM-dd`). Opcional.</param>
    /// <response code="200">Total e faturamento por dia do período.</response>
    [HttpGet("faturamento")]
    [ProducesResponseType(typeof(FaturamentoResumoDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterFaturamento(
        [FromQuery] DateOnly? de,
        [FromQuery] DateOnly? ate)
    {
        var resumo = await _dashboardService.ObterFaturamento(de, ate);

        return Ok(resumo);
    }
}

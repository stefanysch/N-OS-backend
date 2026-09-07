using Microsoft.AspNetCore.Mvc;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("resumo")]
    public async Task<IActionResult> ObterResumo()
    {
        var resumo = await _dashboardService.ObterResumo();

        return Ok(resumo);
    }

    [HttpGet("faturamento")]
    public async Task<IActionResult> ObterFaturamento(
        [FromQuery] DateOnly? de,
        [FromQuery] DateOnly? ate)
    {
        var resumo = await _dashboardService.ObterFaturamento(de, ate);

        return Ok(resumo);
    }
}

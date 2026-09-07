using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/empresa")]
public class EmpresaController : ControllerBase
{
    private readonly IEmpresaService _empresaService;

    public EmpresaController(IEmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<IActionResult> Obter()
    {
        var empresa = await _empresaService.Obter();

        return Ok(empresa);
    }

    [HttpPut]
    public async Task<IActionResult> Atualizar(
        [FromBody] EmpresaUpdateDTO input)
    {
        var empresa = await _empresaService.Atualizar(input);

        return Ok(empresa);
    }
}

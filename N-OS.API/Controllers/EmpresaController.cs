using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

/// <summary>Dados da empresa, exibidos no cabeçalho do PDF da ordem de serviço.</summary>
[ApiController]
[Route("api/empresa")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class EmpresaController : ControllerBase
{
    private readonly IEmpresaService _empresaService;

    public EmpresaController(IEmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    /// <summary>Retorna os dados da empresa.</summary>
    /// <remarks>Existe um único registro. `configurada = false` indica que ainda não foi preenchido.</remarks>
    /// <response code="200">Dados da empresa.</response>
    [HttpGet]
    [ProducesResponseType(typeof(EmpresaResponseDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter()
    {
        var empresa = await _empresaService.Obter();

        return Ok(empresa);
    }

    /// <summary>Atualiza os dados da empresa.</summary>
    /// <remarks>
    /// `documento` (CPF ou CNPJ, com dígitos verificadores) e `telefone`
    /// (10 ou 11 dígitos) são opcionais, mas validados quando informados.
    /// Aceitam máscara, mas são gravados e devolvidos **só com dígitos**.
    /// Campos opcionais vazios são gravados como nulos.
    /// </remarks>
    /// <response code="200">Dados atualizados.</response>
    /// <response code="400">Nome ausente, documento, telefone ou e-mail inválidos.</response>
    [HttpPut]
    [ProducesResponseType(typeof(EmpresaResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar(
        [FromBody] EmpresaUpdateDTO input)
    {
        var empresa = await _empresaService.Atualizar(input);

        return Ok(empresa);
    }
}

using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Infrastructure.Services;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/pecas")]
public class PecaController : ControllerBase
{
    private readonly PecaService _service;

    public PecaController(
        PecaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pecas =
            await _service.Listar();

        return Ok(pecas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var peca =
            await _service.BuscarPorId(id);

        if (peca == null)
            return NotFound("Peça não encontrada");

        return Ok(peca);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] PecaCreateDTO input)
    {
        var peca =
            await _service.Criar(input);

        return CreatedAtAction(
            nameof(GetById),
            new { id = peca.Id },
            peca
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] PecaUpdateDTO input)
    {
        var peca =
            await _service.Atualizar(id, input);

        if (peca == null)
            return NotFound("Peça não encontrada");

        return Ok(peca);
    }

    [HttpPatch("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        var sucesso =
            await _service.Inativar(id);

        if (!sucesso)
            return NotFound("Peça não encontrada");

        return Ok("Peça inativada com sucesso");
    }

    [HttpPatch("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var sucesso =
            await _service.Reativar(id);

        if (!sucesso)
            return NotFound("Peça não encontrada");

        return Ok("Peça reativada com sucesso");
    }
}
using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/pecas")]
public class PecaController : ControllerBase
{
    private readonly IPecaService _pecaService      ;

    public PecaController(IPecaService service)
    {
        _pecaService = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pecas = await _pecaService.Listar();

        return Ok(pecas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var peca = await _pecaService.BuscarPorId(id);

        if (peca == null)
            return NotFound("Peça não encontrada");

        return Ok(peca);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] PecaCreateDTO input)
    {
        var peca = await _pecaService.Criar(input);

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
        var peca = await _pecaService.Atualizar(id, input);

        if (peca == null)
            return NotFound("Peça não encontrada");

        return Ok(peca);
    }

    [HttpPatch("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        var sucesso = await _pecaService.Inativar(id);

        if (!sucesso)
            return NotFound("Peça não encontrada");

        return Ok("Peça inativada com sucesso");
    }

    [HttpPatch("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var sucesso = await _pecaService.Reativar(id);

        if (!sucesso)
            return NotFound("Peça não encontrada");

        return Ok("Peça reativada com sucesso");
    }
}
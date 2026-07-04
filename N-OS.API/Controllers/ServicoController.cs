using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/servicos")]
public class ServicosController : ControllerBase
{
    private readonly IServicoService _servicoService;

    public ServicosController(IServicoService servicoService)
    {
        _servicoService = servicoService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var servicos = await _servicoService.Listar();

        return Ok(servicos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var servico = await _servicoService.BuscarPorId(id);

        if (servico == null)
            return NotFound("Serviço não encontrado.");

        return Ok(servico);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] ServicoCreateDTO input)
    {
        var servico = await _servicoService.Criar(input);

        return CreatedAtAction(
            nameof(GetById),
            new { id = servico.Id },
            servico
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] ServicoUpdateDTO input)
    {
        var servico = await _servicoService.Atualizar(id, input);

        if (servico == null)
            return NotFound("Serviço não encontrado.");

        return Ok(servico);
    }

    [HttpPatch("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        var sucesso = await _servicoService.Inativar(id);

        if (!sucesso)
            return NotFound("Serviço não encontrado.");

        return Ok("Serviço inativado com sucesso.");
    }

    [HttpPatch("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var sucesso = await _servicoService.Reativar(id);

        if (!sucesso)
            return NotFound("Serviço não encontrado.");

        return Ok("Serviço reativado com sucesso.");
    }
}
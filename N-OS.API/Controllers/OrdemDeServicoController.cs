using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/ordens-servico")]
public class OrdemDeServicoController : ControllerBase
{
    private readonly IOrdemDeServicoService _ordemDeServicoService;

    public OrdemDeServicoController(
        IOrdemDeServicoService ordemDeServicoService)
    {
        _ordemDeServicoService = ordemDeServicoService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ordensDeServico =
            await _ordemDeServicoService.Listar();

        return Ok(ordensDeServico);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ordemDeServico =
            await _ordemDeServicoService.BuscarPorId(id);

        if (ordemDeServico == null)
            return NotFound("Ordem de serviço não encontrada");

        return Ok(ordemDeServico);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
    [FromBody] OrdemDeServicoCreateDTO input)
    {
        try
        {
            var ordemDeServico =
                await _ordemDeServicoService.Criar(input);

            return CreatedAtAction(
                nameof(GetById),
                new { id = ordemDeServico.Id },
                ordemDeServico
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] OrdemDeServicoUpdateDTO input)
    {
        var ordemDeServico =
            await _ordemDeServicoService.Atualizar(id, input);

        if (ordemDeServico == null)
            return NotFound("Ordem de serviço não encontrada");

        return Ok(ordemDeServico);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AlterarStatus(
        int id,
        [FromBody] OrdemDeServicoStatusDTO input)
    {
        var ordemDeServico =
            await _ordemDeServicoService.AlterarStatus(id, input);

        if (ordemDeServico == null)
            return NotFound("Ordem de serviço não encontrada");

        return Ok(ordemDeServico);
    }

    [HttpPatch("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        var sucesso =
            await _ordemDeServicoService.Inativar(id);

        if (!sucesso)
            return NotFound("Ordem de serviço não encontrada");

        return Ok("Ordem de serviço inativada com sucesso");
    }

    [HttpPatch("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var sucesso =
            await _ordemDeServicoService.Reativar(id);

        if (!sucesso)
            return NotFound("Ordem de serviço não encontrada");

        return Ok("Ordem de serviço reativada com sucesso");
    }
}
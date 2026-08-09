using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/veiculos")]
public class VeiculoController : ControllerBase
{
    private readonly IVeiculoService _veiculoService;

    public VeiculoController(IVeiculoService service)
    {
        _veiculoService = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var veiculos = await _veiculoService.Listar();

        return Ok(veiculos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var veiculo = await _veiculoService.BuscarPorId(id);

        if (veiculo == null)
            return NotFound("Veículo não encontrado");

        return Ok(veiculo);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] VeiculoCreateDTO input)
    {
        var veiculo = await _veiculoService.Criar(input);

        return CreatedAtAction(
            nameof(GetById),
            new { id = veiculo.Id },
            veiculo
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] VeiculoUpdateDTO input)
    {
        var veiculo = await _veiculoService.Atualizar(id, input);

        if (veiculo == null)
            return NotFound("Veículo não encontrado");

        return Ok(veiculo);
    }

    [HttpPatch("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        var sucesso = await _veiculoService.Inativar(id);

        if (!sucesso)
            return NotFound("Veículo não encontrado");

        return Ok("Veículo inativado com sucesso");
    }

    [HttpPatch("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var sucesso = await _veiculoService.Reativar(id);

        if (!sucesso)
            return NotFound("Veículo não encontrado");

        return Ok("Veículo reativado com sucesso");
    }
}
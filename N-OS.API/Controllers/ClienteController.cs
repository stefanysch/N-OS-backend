using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService service)
    {
        _clienteService = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var clientes = await _clienteService.Listar();

        return Ok(clientes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _clienteService.BuscarPorId(id);

        if (cliente == null)
            return NotFound("Cliente não encontrado");

        return Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] ClienteCreateDTO input)
    {
        try
        {
            var cliente = await _clienteService.Criar(input);

            return CreatedAtAction(
                nameof(GetById),
                new { id = cliente.Id },
                cliente
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
        [FromBody] ClienteUpdateDTO input)
    {
        try
        {
            var cliente = await _clienteService.Atualizar(id, input);

            if (cliente == null)
                return NotFound("Cliente não encontrado");

            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPatch("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        try
        {
            var sucesso = await _clienteService.Inativar(id);

            if (!sucesso)
                return NotFound("Cliente não encontrado");

            return Ok("Cliente inativado com sucesso");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPatch("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var sucesso = await _clienteService.Reativar(id);

        if (!sucesso)
            return NotFound("Cliente não encontrado");

        return Ok("Cliente reativado com sucesso");
    }
}
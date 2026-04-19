using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClienteController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClienteController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/clientes
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var clientes = await _context.Clientes.ToListAsync();
        return Ok(clientes);
    }

    // GET: api/clientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return NotFound("Cliente não encontrado");

        return Ok(cliente);
    }

    // POST: api/clientes
    [HttpPost]
    public async Task<IActionResult> Post(ClienteCreateDTO input)
    {
        if (string.IsNullOrWhiteSpace(input.Nome) ||
            string.IsNullOrWhiteSpace(input.Telefone))
        {
            return BadRequest("Nome e Telefone são obrigatórios");
        }

        var cliente = new Cliente
        {
            Nome = input.Nome,
            Telefone = input.Telefone,
            Documento = input.Documento ?? string.Empty,
            Email = input.Email ?? string.Empty,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return Ok(cliente);
    }

    // PUT: api/clientes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, ClienteUpdateDTO input)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return NotFound("Cliente não encontrado");

        if (string.IsNullOrWhiteSpace(input.Nome) ||
            string.IsNullOrWhiteSpace(input.Telefone))
        {
            return BadRequest("Nome e Telefone são obrigatórios");
        }

        cliente.Nome = input.Nome;
        cliente.Telefone = input.Telefone;
        cliente.Documento = input.Documento;
        cliente.Email = input.Email;

        await _context.SaveChangesAsync();

        return Ok(cliente);
    }

    // PUT: api/clientes/inativar/{id}
    [HttpPut("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return NotFound("Cliente não encontrado");

        cliente.Ativo = false;

        await _context.SaveChangesAsync();

        return Ok("Cliente inativado com sucesso");
    }

    // PUT: api/clientes/reativar/{id}
    [HttpPut("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return NotFound("Cliente não encontrado");

        cliente.Ativo = true;

        await _context.SaveChangesAsync();

        return Ok("Cliente reativado com sucesso");
    }
}
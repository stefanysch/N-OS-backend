using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/servicos")]
public class ServicosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServicosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/servicos
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var servicos = await _context.Servicos.ToListAsync();
        return Ok(servicos);
    }

    // GET: api/servicos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var servico = await _context.Servicos.FindAsync(id);

        if (servico == null)
            return NotFound("Serviço não encontrado");

        return Ok(servico);
    }

    // POST: api/servicos
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ServicoCreateDTO input)
    {
        var servico = new Servico
        {
            Nome = input.Nome,
            Descricao = input.Descricao,
            Valor = input.Valor,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        _context.Servicos.Add(servico);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = servico.Id }, servico);
    }

    // PUT: api/servicos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] ServicoUpdateDTO input)
    {
        var servico = await _context.Servicos.FindAsync(id);

        if (servico == null)
            return NotFound("Serviço não encontrado");

        servico.Nome = input.Nome;
        servico.Descricao = input.Descricao;
        servico.Valor = input.Valor;

        await _context.SaveChangesAsync();

        return Ok(servico);
    }

    // PATCH: api/servicos/inativar/{id}
    [HttpPatch("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        var servico = await _context.Servicos.FindAsync(id);

        if (servico == null)
            return NotFound("Serviço não encontrado");

        servico.Ativo = false;

        await _context.SaveChangesAsync();

        return Ok("Serviço inativado com sucesso");
    }

    // PATCH: api/servicos/reativar/{id}
    [HttpPatch("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var servico = await _context.Servicos.FindAsync(id);

        if (servico == null)
            return NotFound("Serviço não encontrado");

        servico.Ativo = true;

        await _context.SaveChangesAsync();

        return Ok("Serviço reativado com sucesso");
    }
} 
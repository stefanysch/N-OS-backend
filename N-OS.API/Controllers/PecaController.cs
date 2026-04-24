using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N_OS.Application.DTOs;
using N_OS.Domain.Entities;
using N_OS.Infrastructure.Data;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/pecas")]
public class PecaController : ControllerBase
{
    private readonly AppDbContext _context;

    public PecaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/pecas
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pecas = await _context.Pecas.ToListAsync();
        return Ok(pecas);
    }

    // GET: api/pecas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var peca = await _context.Pecas.FindAsync(id);

        if (peca == null)
            return NotFound("Peça não encontrada");

        return Ok(peca);
    }

    // POST: api/pecas
    [HttpPost]
    public async Task<IActionResult> Post(PecaCreateDTO input)
    {
        if (string.IsNullOrWhiteSpace(input.Nome))
            return BadRequest("Nome é obrigatório");

        var peca = new Peca
        {
            Nome = input.Nome,
            Descricao = input.Descricao,
            Valor = input.Valor,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        _context.Pecas.Add(peca);
        await _context.SaveChangesAsync();

        return Ok(peca);
    }

    // PUT: api/pecas/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, PecaUpdateDTO input)
    {
        var peca = await _context.Pecas.FindAsync(id);

        if (peca == null)
            return NotFound("Peça não encontrada");

        peca.Nome = input.Nome;
        peca.Descricao = input.Descricao;
        peca.Valor = input.Valor;

        await _context.SaveChangesAsync();

        return Ok(peca);
    }

    // PATCH: api/pecas/inativar/{id}
    [HttpPatch("inativar/{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        var peca = await _context.Pecas.FindAsync(id);

        if (peca == null)
            return NotFound("Peça não encontrada");

        peca.Ativo = false;

        await _context.SaveChangesAsync();

        return Ok("Peça inativada com sucesso");
    }

    // Patch: api/pecas/reativar/{id}
    [HttpPatch("reativar/{id}")]
    public async Task<IActionResult> Reativar(int id)
    {
        var peca = await _context.Pecas.FindAsync(id);

        if (peca == null)
            return NotFound("Peça não encontrada");

        peca.Ativo = true;

        await _context.SaveChangesAsync();

        return Ok("Peça reativada com sucesso");
    }
}
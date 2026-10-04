using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Exceptions;
using N_OS.Application.Interfaces;
using N_OS.Application.Pagination;
using N_OS.Domain.Entities;

namespace N_OS.API.Controllers;

/// <summary>Catálogo de peças usadas nas ordens de serviço.</summary>
[ApiController]
[Route("api/pecas")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class PecaController : ControllerBase
{
    private readonly IPecaService _pecaService;
    private readonly IPecaQueries _pecaQueries;

    public PecaController(IPecaService service, IPecaQueries queries)
    {
        _pecaService = service;
        _pecaQueries = queries;
    }

    /// <summary>Lista peças paginadas.</summary>
    /// <remarks>
    /// Ativas primeiro e inativas por último. `q` busca por nome ou descrição.
    /// `sort`: `nome` (padrão), `valor` ou `criadoEm`. `dir`: `asc` ou `desc`. `pageSize` máximo de 100.
    /// </remarks>
    /// <response code="200">Página de peças (pode ter zero itens).</response>
    [HttpGet("paginado")]
    [ProducesResponseType(typeof(PagedResult<PecaListagemDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginado([FromQuery] PageQuery query)
    {
        var pagina = await _pecaQueries.Listar(query);

        return Ok(pagina);
    }

    /// <summary>Lista todas as peças.</summary>
    /// <remarks>Retorna ativas e inativas. Não há paginação.</remarks>
    /// <response code="200">Lista de peças (pode ser vazia).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Peca>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var pecas = await _pecaService.Listar();

        return Ok(pecas);
    }

    /// <summary>Busca uma peça pelo id.</summary>
    /// <param name="id">Id da peça.</param>
    /// <response code="200">Peça encontrada.</response>
    /// <response code="404">Peça não existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Peca), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var peca = await _pecaService.BuscarPorId(id)
            ?? throw new NaoEncontradoException("Peça não encontrada");

        return Ok(peca);
    }

    /// <summary>Cadastra uma peça.</summary>
    /// <response code="201">Peça criada. O header `Location` aponta para o recurso.</response>
    /// <response code="400">Campos inválidos (ex.: valor menor ou igual a zero).</response>
    [HttpPost]
    [ProducesResponseType(typeof(Peca), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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

    /// <summary>Atualiza uma peça.</summary>
    /// <remarks>
    /// Alterar o valor não muda ordens de serviço já abertas,
    /// que guardam o valor aplicado no momento em que o item foi adicionado.
    /// </remarks>
    /// <param name="id">Id da peça.</param>
    /// <response code="200">Peça atualizada.</response>
    /// <response code="400">Campos inválidos.</response>
    /// <response code="404">Peça não existe.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Peca), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] PecaUpdateDTO input)
    {
        var peca = await _pecaService.Atualizar(id, input)
            ?? throw new NaoEncontradoException("Peça não encontrada");

        return Ok(peca);
    }

    /// <summary>Inativa uma peça.</summary>
    /// <param name="id">Id da peça.</param>
    /// <response code="200">Peça inativada.</response>
    /// <response code="404">Peça não existe.</response>
    [HttpPatch("inativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        if (!await _pecaService.Inativar(id))
            throw new NaoEncontradoException("Peça não encontrada");

        return Ok(new MensagemResponseDTO("Peça inativada com sucesso"));
    }

    /// <summary>Reativa uma peça.</summary>
    /// <param name="id">Id da peça.</param>
    /// <response code="200">Peça reativada.</response>
    /// <response code="404">Peça não existe.</response>
    [HttpPatch("reativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reativar(int id)
    {
        if (!await _pecaService.Reativar(id))
            throw new NaoEncontradoException("Peça não encontrada");

        return Ok(new MensagemResponseDTO("Peça reativada com sucesso"));
    }
}

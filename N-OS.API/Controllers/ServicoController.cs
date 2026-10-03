using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Exceptions;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;

namespace N_OS.API.Controllers;

/// <summary>Catálogo de serviços (mão de obra) usados nas ordens de serviço.</summary>
[ApiController]
[Route("api/servicos")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class ServicoController : ControllerBase
{
    private readonly IServicoService _servicoService;

    public ServicoController(IServicoService servicoService)
    {
        _servicoService = servicoService;
    }

    /// <summary>Lista todos os serviços.</summary>
    /// <remarks>Retorna ativos e inativos. Não há paginação.</remarks>
    /// <response code="200">Lista de serviços (pode ser vazia).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Servico>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var servicos = await _servicoService.Listar();

        return Ok(servicos);
    }

    /// <summary>Busca um serviço pelo id.</summary>
    /// <param name="id">Id do serviço.</param>
    /// <response code="200">Serviço encontrado.</response>
    /// <response code="404">Serviço não existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Servico), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var servico = await _servicoService.BuscarPorId(id)
            ?? throw new NaoEncontradoException("Serviço não encontrado");

        return Ok(servico);
    }

    /// <summary>Cadastra um serviço.</summary>
    /// <response code="201">Serviço criado. O header `Location` aponta para o recurso.</response>
    /// <response code="400">Campos inválidos (ex.: valor menor ou igual a zero).</response>
    [HttpPost]
    [ProducesResponseType(typeof(Servico), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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

    /// <summary>Atualiza um serviço.</summary>
    /// <remarks>
    /// Alterar o valor não muda ordens de serviço já abertas,
    /// que guardam o valor aplicado no momento em que o item foi adicionado.
    /// </remarks>
    /// <param name="id">Id do serviço.</param>
    /// <response code="200">Serviço atualizado.</response>
    /// <response code="400">Campos inválidos.</response>
    /// <response code="404">Serviço não existe.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Servico), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] ServicoUpdateDTO input)
    {
        var servico = await _servicoService.Atualizar(id, input)
            ?? throw new NaoEncontradoException("Serviço não encontrado");

        return Ok(servico);
    }

    /// <summary>Inativa um serviço.</summary>
    /// <param name="id">Id do serviço.</param>
    /// <response code="200">Serviço inativado.</response>
    /// <response code="404">Serviço não existe.</response>
    [HttpPatch("inativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        if (!await _servicoService.Inativar(id))
            throw new NaoEncontradoException("Serviço não encontrado");

        return Ok(new MensagemResponseDTO("Serviço inativado com sucesso"));
    }

    /// <summary>Reativa um serviço.</summary>
    /// <param name="id">Id do serviço.</param>
    /// <response code="200">Serviço reativado.</response>
    /// <response code="404">Serviço não existe.</response>
    [HttpPatch("reativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reativar(int id)
    {
        if (!await _servicoService.Reativar(id))
            throw new NaoEncontradoException("Serviço não encontrado");

        return Ok(new MensagemResponseDTO("Serviço reativado com sucesso"));
    }
}

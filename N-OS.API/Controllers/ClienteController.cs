using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Exceptions;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

/// <summary>Cadastro de clientes.</summary>
[ApiController]
[Route("api/clientes")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService service)
    {
        _clienteService = service;
    }

    /// <summary>Lista todos os clientes.</summary>
    /// <remarks>Retorna ativos e inativos. Não há paginação.</remarks>
    /// <response code="200">Lista de clientes (pode ser vazia).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var clientes = await _clienteService.Listar();

        return Ok(clientes);
    }

    /// <summary>Busca um cliente pelo id.</summary>
    /// <remarks>A resposta inclui a lista de veículos do cliente.</remarks>
    /// <param name="id">Id do cliente.</param>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="404">Cliente não existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _clienteService.BuscarPorId(id)
            ?? throw new NaoEncontradoException("Cliente não encontrado");

        return Ok(cliente);
    }

    /// <summary>Cadastra um cliente.</summary>
    /// <remarks>
    /// O documento (CPF/CNPJ) é validado e deve ser único.
    /// O endereço é opcional, mas se algum campo for informado,
    /// CEP, logradouro, número, bairro, cidade e UF passam a ser obrigatórios.
    /// `tipoDocumento`: 1 = CPF, 2 = CNPJ.
    /// </remarks>
    /// <response code="201">Cliente criado. O header `Location` aponta para o recurso.</response>
    /// <response code="400">Campos inválidos, documento inválido ou já cadastrado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromBody] ClienteCreateDTO input)
    {
        var cliente = await _clienteService.Criar(input);

        return CreatedAtAction(
            nameof(GetById),
            new { id = cliente.Id },
            cliente
        );
    }

    /// <summary>Atualiza os dados de um cliente.</summary>
    /// <remarks>Mesmas regras do cadastro. O corpo substitui todos os campos.</remarks>
    /// <param name="id">Id do cliente.</param>
    /// <response code="200">Cliente atualizado.</response>
    /// <response code="400">Campos inválidos, documento inválido ou já cadastrado.</response>
    /// <response code="404">Cliente não existe.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] ClienteUpdateDTO input)
    {
        var cliente = await _clienteService.Atualizar(id, input)
            ?? throw new NaoEncontradoException("Cliente não encontrado");

        return Ok(cliente);
    }

    /// <summary>Inativa um cliente.</summary>
    /// <remarks>Os veículos do cliente também são inativados.</remarks>
    /// <param name="id">Id do cliente.</param>
    /// <response code="200">Cliente inativado.</response>
    /// <response code="404">Cliente não existe.</response>
    /// <response code="409">Algum veículo do cliente tem ordem de serviço ativa.</response>
    [HttpPatch("inativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Inativar(int id)
    {
        if (!await _clienteService.Inativar(id))
            throw new NaoEncontradoException("Cliente não encontrado");

        return Ok(new MensagemResponseDTO("Cliente inativado com sucesso"));
    }

    /// <summary>Reativa um cliente.</summary>
    /// <param name="id">Id do cliente.</param>
    /// <response code="200">Cliente reativado.</response>
    /// <response code="404">Cliente não existe.</response>
    [HttpPatch("reativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reativar(int id)
    {
        if (!await _clienteService.Reativar(id))
            throw new NaoEncontradoException("Cliente não encontrado");

        return Ok(new MensagemResponseDTO("Cliente reativado com sucesso"));
    }
}

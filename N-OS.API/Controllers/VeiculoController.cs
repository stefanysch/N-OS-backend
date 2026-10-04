using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Exceptions;
using N_OS.Application.Interfaces;
using N_OS.Application.Pagination;

namespace N_OS.API.Controllers;

/// <summary>Cadastro de veículos.</summary>
[ApiController]
[Route("api/veiculos")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class VeiculoController : ControllerBase
{
    private readonly IVeiculoService _veiculoService;
    private readonly IVeiculoQueries _veiculoQueries;

    public VeiculoController(IVeiculoService service, IVeiculoQueries queries)
    {
        _veiculoService = service;
        _veiculoQueries = queries;
    }

    /// <summary>Lista veículos paginados.</summary>
    /// <remarks>
    /// Ativos primeiro e inativos por último. `q` busca por placa, marca, modelo ou nome do cliente.
    /// `sort`: `placa` (padrão), `marca`, `modelo`, `ano`, `cliente` ou `criadoEm`.
    /// `dir`: `asc` ou `desc`. `pageSize` máximo de 100.
    /// </remarks>
    /// <response code="200">Página de veículos (pode ter zero itens).</response>
    [HttpGet("paginado")]
    [ProducesResponseType(typeof(PagedResult<VeiculoListagemDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginado([FromQuery] PageQuery query)
    {
        var pagina = await _veiculoQueries.Listar(query);

        return Ok(pagina);
    }

    /// <summary>Lista todos os veículos.</summary>
    /// <remarks>Retorna ativos e inativos. Não há paginação.</remarks>
    /// <response code="200">Lista de veículos (pode ser vazia).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VeiculoResponseDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var veiculos = await _veiculoService.Listar();

        return Ok(veiculos);
    }

    /// <summary>Lista os veículos de um cliente.</summary>
    /// <param name="clienteId">Id do cliente.</param>
    /// <response code="200">Veículos do cliente (lista vazia se não houver).</response>
    [HttpGet("cliente/{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<VeiculoResponseDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCliente(int clienteId)
    {
        var veiculos = await _veiculoService.ListarPorCliente(clienteId);

        return Ok(veiculos);
    }

    /// <summary>Busca um veículo pelo id.</summary>
    /// <param name="id">Id do veículo.</param>
    /// <response code="200">Veículo encontrado.</response>
    /// <response code="404">Veículo não existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VeiculoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var veiculo = await _veiculoService.BuscarPorId(id)
            ?? throw new NaoEncontradoException("Veículo não encontrado");

        return Ok(veiculo);
    }

    /// <summary>Cadastra um veículo.</summary>
    /// <remarks>
    /// O cliente informado precisa existir e estar ativo.
    /// A placa aceita o padrão antigo (ABC-1234) ou Mercosul (ABC1D23), com ou sem hífen.
    /// </remarks>
    /// <response code="201">Veículo criado. O header `Location` aponta para o recurso.</response>
    /// <response code="400">Campos inválidos (inclusive placa), cliente inexistente ou inativo.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VeiculoResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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

    /// <summary>Atualiza os dados de um veículo.</summary>
    /// <remarks>O corpo substitui todos os campos.</remarks>
    /// <param name="id">Id do veículo.</param>
    /// <response code="200">Veículo atualizado.</response>
    /// <response code="400">Campos inválidos, cliente inexistente ou inativo.</response>
    /// <response code="404">Veículo não existe.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VeiculoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] VeiculoUpdateDTO input)
    {
        var veiculo = await _veiculoService.Atualizar(id, input)
            ?? throw new NaoEncontradoException("Veículo não encontrado");

        return Ok(veiculo);
    }

    /// <summary>Inativa um veículo.</summary>
    /// <param name="id">Id do veículo.</param>
    /// <response code="200">Veículo inativado.</response>
    /// <response code="400">O veículo possui ordem de serviço ativa.</response>
    /// <response code="404">Veículo não existe.</response>
    [HttpPatch("inativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        if (!await _veiculoService.Inativar(id))
            throw new NaoEncontradoException("Veículo não encontrado");

        return Ok(new MensagemResponseDTO("Veículo inativado com sucesso"));
    }

    /// <summary>Reativa um veículo.</summary>
    /// <param name="id">Id do veículo.</param>
    /// <response code="200">Veículo reativado.</response>
    /// <response code="400">O cliente do veículo está inativo. Reative o cliente primeiro.</response>
    /// <response code="404">Veículo não existe.</response>
    [HttpPatch("reativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reativar(int id)
    {
        if (!await _veiculoService.Reativar(id))
            throw new NaoEncontradoException("Veículo não encontrado");

        return Ok(new MensagemResponseDTO("Veículo reativado com sucesso"));
    }
}

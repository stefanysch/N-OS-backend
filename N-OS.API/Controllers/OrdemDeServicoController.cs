using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Exceptions;
using N_OS.Application.Interfaces;
using N_OS.Application.Pagination;

namespace N_OS.API.Controllers;

/// <summary>Ordens de serviço e seus itens (peças e serviços).</summary>
[ApiController]
[Route("api/ordens-servico")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class OrdemDeServicoController : ControllerBase
{
    private const string NaoEncontrada = "Ordem de serviço não encontrada";

    private readonly IOrdemDeServicoService _ordemDeServicoService;
    private readonly IOrdemDeServicoQueries _ordemDeServicoQueries;

    public OrdemDeServicoController(
        IOrdemDeServicoService ordemDeServicoService,
        IOrdemDeServicoQueries ordemDeServicoQueries)
    {
        _ordemDeServicoService = ordemDeServicoService;
        _ordemDeServicoQueries = ordemDeServicoQueries;
    }

    /// <summary>Lista ordens de serviço paginadas, sem itens.</summary>
    /// <remarks>
    /// Ativas primeiro e inativas por último. `q` busca por nome do cliente ou placa.
    /// `sort`: `cliente`, `placa`, `status`, `valorTotal` ou `dataAbertura` (padrão, decrescente).
    /// `dir`: `asc` ou `desc`. `pageSize` máximo de 100.
    /// </remarks>
    /// <response code="200">Página de ordens (pode ter zero itens).</response>
    [HttpGet("paginado")]
    [ProducesResponseType(typeof(PagedResult<OrdemDeServicoListagemDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginado([FromQuery] PageQuery query)
    {
        var pagina = await _ordemDeServicoQueries.Listar(query);

        return Ok(pagina);
    }

    /// <summary>Lista todas as ordens de serviço.</summary>
    /// <remarks>Retorna ativas e inativas. Não há paginação.</remarks>
    /// <response code="200">Lista de ordens (pode ser vazia).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrdemDeServicoResponseDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var ordensDeServico =
            await _ordemDeServicoService.Listar();

        return Ok(ordensDeServico);
    }

    /// <summary>Busca uma ordem de serviço pelo id.</summary>
    /// <remarks>A resposta inclui os itens da ordem.</remarks>
    /// <param name="id">Id da ordem de serviço.</param>
    /// <response code="200">Ordem encontrada.</response>
    /// <response code="404">Ordem não existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrdemDeServicoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var ordemDeServico =
            await _ordemDeServicoService.BuscarPorId(id)
            ?? throw new NaoEncontradoException(NaoEncontrada);

        return Ok(ordemDeServico);
    }

    /// <summary>Abre uma ordem de serviço.</summary>
    /// <remarks>
    /// O veículo e o cliente dele precisam estar ativos.
    /// A ordem precisa ter ao menos um item, e cada item precisa de `pecaId` ou `servicoId`.
    /// O valor de cada item é congelado no momento em que ele entra na ordem.
    /// O desconto não pode ser negativo nem maior que o valor dos itens.
    /// </remarks>
    /// <response code="201">Ordem criada. O header `Location` aponta para o recurso.</response>
    /// <response code="400">Campos inválidos, veículo ou cliente inativo, item inválido ou desconto acima do total.</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrdemDeServicoResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromBody] OrdemDeServicoCreateDTO input)
    {
        var ordemDeServico =
            await _ordemDeServicoService.Criar(input);

        return CreatedAtAction(
            nameof(GetById),
            new { id = ordemDeServico.Id },
            ordemDeServico
        );
    }

    /// <summary>Atualiza descrição, observações e desconto, e adiciona itens.</summary>
    /// <remarks>
    /// Os itens enviados em `itens` são **adicionados** à ordem; os já existentes
    /// não são alterados. Para remover um item use `DELETE /{id}/itens/{itemId}`.
    /// </remarks>
    /// <param name="id">Id da ordem de serviço.</param>
    /// <response code="200">Ordem atualizada.</response>
    /// <response code="400">Campos inválidos, item inválido ou desconto acima do total.</response>
    /// <response code="404">Ordem não existe.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrdemDeServicoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] OrdemDeServicoUpdateDTO input)
    {
        var ordemDeServico =
            await _ordemDeServicoService.Atualizar(id, input)
            ?? throw new NaoEncontradoException(NaoEncontrada);

        return Ok(ordemDeServico);
    }

    /// <summary>Altera o status da ordem de serviço.</summary>
    /// <remarks>
    /// Valores de `status`: 0 = Aguardando, 1 = AguardandoPecas, 2 = EmExecucao,
    /// 3 = EmTeste, 4 = Concluida.
    /// </remarks>
    /// <param name="id">Id da ordem de serviço.</param>
    /// <response code="200">Ordem com o novo status.</response>
    /// <response code="400">Status inválido.</response>
    /// <response code="404">Ordem não existe.</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(OrdemDeServicoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarStatus(
        int id,
        [FromBody] OrdemDeServicoStatusDTO input)
    {
        var ordemDeServico =
            await _ordemDeServicoService.AlterarStatus(id, input)
            ?? throw new NaoEncontradoException(NaoEncontrada);

        return Ok(ordemDeServico);
    }

    /// <summary>Remove um item da ordem de serviço.</summary>
    /// <remarks>A ordem precisa manter ao menos um item: remover o último é recusado.</remarks>
    /// <param name="id">Id da ordem de serviço.</param>
    /// <param name="itemId">Id do item a remover.</param>
    /// <response code="200">Ordem atualizada, já sem o item.</response>
    /// <response code="400">Item não pertence à ordem ou é o último item.</response>
    /// <response code="404">Ordem não existe.</response>
    [HttpDelete("{id}/itens/{itemId}")]
    [ProducesResponseType(typeof(OrdemDeServicoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverItem(int id, int itemId)
    {
        var ordemDeServico =
            await _ordemDeServicoService.RemoverItem(id, itemId)
            ?? throw new NaoEncontradoException(NaoEncontrada);

        return Ok(ordemDeServico);
    }

    /// <summary>Inativa uma ordem de serviço.</summary>
    /// <param name="id">Id da ordem de serviço.</param>
    /// <response code="200">Ordem inativada.</response>
    /// <response code="404">Ordem não existe.</response>
    [HttpPatch("inativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        if (!await _ordemDeServicoService.Inativar(id))
            throw new NaoEncontradoException(NaoEncontrada);

        return Ok(new MensagemResponseDTO("Ordem de serviço inativada com sucesso"));
    }

    /// <summary>Reativa uma ordem de serviço.</summary>
    /// <param name="id">Id da ordem de serviço.</param>
    /// <response code="200">Ordem reativada.</response>
    /// <response code="400">O veículo ou o cliente da ordem está inativo. Reative-o primeiro.</response>
    /// <response code="404">Ordem não existe.</response>
    [HttpPatch("reativar/{id}")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reativar(int id)
    {
        if (!await _ordemDeServicoService.Reativar(id))
            throw new NaoEncontradoException(NaoEncontrada);

        return Ok(new MensagemResponseDTO("Ordem de serviço reativada com sucesso"));
    }
}

using N_OS.Application.DTOs;
using N_OS.Application.Pagination;

namespace N_OS.Application.Interfaces;

public interface IOrdemDeServicoQueries
{
    Task<PagedResult<OrdemDeServicoListagemDTO>> Listar(PageQuery query);
}

using N_OS.Application.DTOs;
using N_OS.Application.Pagination;

namespace N_OS.Application.Interfaces;

public interface IServicoQueries
{
    Task<PagedResult<ServicoListagemDTO>> Listar(PageQuery query);
}

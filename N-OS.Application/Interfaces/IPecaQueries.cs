using N_OS.Application.DTOs;
using N_OS.Application.Pagination;

namespace N_OS.Application.Interfaces;

public interface IPecaQueries
{
    Task<PagedResult<PecaListagemDTO>> Listar(PageQuery query);
}

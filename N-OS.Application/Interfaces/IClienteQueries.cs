using N_OS.Application.DTOs;
using N_OS.Application.Pagination;

namespace N_OS.Application.Interfaces;

public interface IClienteQueries
{
    Task<PagedResult<ClienteListagemDTO>> Listar(PageQuery query);
}

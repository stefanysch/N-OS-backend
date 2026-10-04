using N_OS.Application.DTOs;
using N_OS.Application.Pagination;

namespace N_OS.Application.Interfaces;

public interface IVeiculoQueries
{
    Task<PagedResult<VeiculoListagemDTO>> Listar(PageQuery query);
}

using N_OS.Domain.Entities;

namespace N_OS.Domain.Interfaces;

public interface IOrdemDeServicoRepository
{
    Task<IEnumerable<OrdemDeServico>> Listar();

    Task<OrdemDeServico?> BuscarPorId(int id);

    Task Criar(OrdemDeServico ordemDeServico);

    Task Atualizar(OrdemDeServico ordemDeServico);

    Task SaveChanges();
}
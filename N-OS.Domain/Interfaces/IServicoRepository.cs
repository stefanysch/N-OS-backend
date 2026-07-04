using N_OS.Domain.Entities;

namespace N_OS.Domain.Interfaces;

public interface IServicoRepository
{
    Task<List<Servico>> Listar();

    Task<Servico?> BuscarPorId(int id);

    Task<Servico> Criar(Servico servico);

    Task<Servico?> Atualizar(Servico servico);

    Task SaveChanges();
}
using N_OS.Domain.Entities;

namespace N_OS.Domain.Interfaces;

public interface IVeiculoRepository
{
    Task<IEnumerable<Veiculo>> Listar();

    Task<IEnumerable<Veiculo>> ListarPorCliente(int clienteId);

    Task<Veiculo?> BuscarPorId(int id);

    Task Criar(Veiculo veiculo);

    Task Atualizar(Veiculo veiculo);

    Task SaveChanges();
}
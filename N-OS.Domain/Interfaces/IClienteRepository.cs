using N_OS.Domain.Entities;

namespace N_OS.Domain.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> Listar();

    Task<Cliente?> BuscarPorId(int id);

    Task<Cliente?> BuscarPorDocumento(string numero);

    Task Criar(Cliente cliente);

    Task Atualizar(Cliente cliente);

    Task<IEnumerable<Veiculo>> ListarVeiculos(int clienteId);

    Task<IEnumerable<string>> PlacasDeVeiculosComOSAtiva(int clienteId);

    Task SaveChanges();
}
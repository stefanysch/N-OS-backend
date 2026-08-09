using N_OS.Domain.Entities;

namespace N_OS.Domain.Interfaces;

public interface IPecaRepository
{
    Task<IEnumerable<Peca>> Listar();

    Task<Peca?> BuscarPorId(int id);

    Task<Peca> Criar(Peca peca);

    Task<Peca?> Atualizar(Peca peca);

    Task SaveChanges();
}
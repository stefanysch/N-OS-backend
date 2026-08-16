using N_OS.Domain.Entities;

namespace N_OS.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorId(int id);

    Task<Usuario?> BuscarPorEmail(string email);

    Task Criar(Usuario usuario);

    Task SaveChanges();
}

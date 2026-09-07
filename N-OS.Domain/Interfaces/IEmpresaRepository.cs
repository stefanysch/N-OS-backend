using N_OS.Domain.Entities;

namespace N_OS.Domain.Interfaces;

public interface IEmpresaRepository
{
    Task<Empresa?> Obter();

    Task Criar(Empresa empresa);

    Task Atualizar(Empresa empresa);

    Task SaveChanges();
}

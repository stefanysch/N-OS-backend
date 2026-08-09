using N_OS.Application.DTOs;
using N_OS.Domain.Entities;

namespace N_OS.Application.Interfaces;

public interface IServicoService
{
    Task<IEnumerable<Servico>> Listar();

    Task<Servico?> BuscarPorId(int id);

    Task<Servico> Criar(ServicoCreateDTO input);

    Task<Servico?> Atualizar(int id, ServicoUpdateDTO input);

    Task<bool> Inativar(int id);

    Task<bool> Reativar(int id);
}
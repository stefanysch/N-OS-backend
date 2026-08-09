using N_OS.Application.DTOs;
using N_OS.Domain.Entities;

namespace N_OS.Application.Interfaces;

public interface IPecaService
{
    Task<IEnumerable<Peca>> Listar();

    Task<Peca?> BuscarPorId(int id);

    Task<Peca> Criar(PecaCreateDTO input);

    Task<Peca?> Atualizar(int id, PecaUpdateDTO input);

    Task<bool> Inativar(int id);

    Task<bool> Reativar(int id);
}
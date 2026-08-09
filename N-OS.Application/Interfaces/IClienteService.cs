using N_OS.Application.DTOs;

namespace N_OS.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteResponseDTO>> Listar();

    Task<ClienteResponseDTO?> BuscarPorId(int id);

    Task<ClienteResponseDTO> Criar(ClienteCreateDTO input);

    Task<ClienteResponseDTO?> Atualizar(
        int id,
        ClienteUpdateDTO input);

    Task<bool> Inativar(int id);

    Task<bool> Reativar(int id);
}
using N_OS.Application.DTOs;

namespace N_OS.Application.Interfaces;

public interface IVeiculoService
{
    Task<IEnumerable<VeiculoResponseDTO>> Listar();

    Task<IEnumerable<VeiculoResponseDTO>> ListarPorCliente(int clienteId);

    Task<VeiculoResponseDTO?> BuscarPorId(int id);

    Task<VeiculoResponseDTO> Criar(VeiculoCreateDTO input);

    Task<VeiculoResponseDTO?> Atualizar(
        int id,
        VeiculoUpdateDTO input);

    Task<bool> Inativar(int id);

    Task<bool> Reativar(int id);
}
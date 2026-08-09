using N_OS.Application.DTOs;

namespace N_OS.Application.Interfaces;

public interface IOrdemDeServicoService
{
    Task<IEnumerable<OrdemDeServicoResponseDTO>> Listar();

Task<OrdemDeServicoResponseDTO?> BuscarPorId(int id);

    Task<OrdemDeServicoResponseDTO> Criar(
        OrdemDeServicoCreateDTO input);

    Task<OrdemDeServicoResponseDTO?> Atualizar(
        int id,
        OrdemDeServicoUpdateDTO input);

    Task<OrdemDeServicoResponseDTO?> AlterarStatus(
        int id,
        OrdemDeServicoStatusDTO input);

    Task<bool> Inativar(int id);

    Task<bool> Reativar(int id);
}
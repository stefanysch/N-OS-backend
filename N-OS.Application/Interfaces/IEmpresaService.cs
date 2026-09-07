using N_OS.Application.DTOs;

namespace N_OS.Application.Interfaces;

public interface IEmpresaService
{
    Task<EmpresaResponseDTO> Obter();

    Task<EmpresaResponseDTO> Atualizar(EmpresaUpdateDTO input);
}

using N_OS.Application.DTOs;

namespace N_OS.Application.Interfaces;

public interface IAuthService
{
    Task<UsuarioResponseDTO> Registrar(RegistrarUsuarioDTO input);

    Task<LoginResponseDTO> Login(LoginDTO input);
}

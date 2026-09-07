using N_OS.Application.DTOs;

namespace N_OS.Application.Interfaces;

public interface IPerfilService
{
    Task<UsuarioResponseDTO> Obter(int usuarioId);

    Task<UsuarioResponseDTO> Atualizar(int usuarioId, AtualizarPerfilDTO input);

    Task AlterarSenha(int usuarioId, AlterarSenhaDTO input);
}

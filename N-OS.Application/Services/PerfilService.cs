using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class PerfilService : IPerfilService
{
    private readonly IUsuarioRepository _repository;

    public PerfilService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<UsuarioResponseDTO> Obter(int usuarioId)
    {
        var usuario = await _repository.BuscarPorId(usuarioId);

        if (usuario == null)
        {
            throw new ArgumentException("Usuário não encontrado.");
        }

        return MapearParaResponse(usuario);
    }

    public async Task<UsuarioResponseDTO> Atualizar(
        int usuarioId,
        AtualizarPerfilDTO input)
    {
        var usuario = await _repository.BuscarPorId(usuarioId);

        if (usuario == null)
        {
            throw new ArgumentException("Usuário não encontrado.");
        }

        var emailEmUso = await _repository.BuscarPorEmail(input.Email);

        if (emailEmUso != null && emailEmUso.Id != usuarioId)
        {
            throw new ArgumentException(
                "Já existe um usuário cadastrado com este e-mail.");
        }

        usuario.Nome = input.Nome;
        usuario.Email = input.Email;

        await _repository.Atualizar(usuario);
        await _repository.SaveChanges();

        return MapearParaResponse(usuario);
    }

    public async Task AlterarSenha(int usuarioId, AlterarSenhaDTO input)
    {
        var usuario = await _repository.BuscarPorId(usuarioId);

        if (usuario == null)
        {
            throw new ArgumentException("Usuário não encontrado.");
        }

        if (!BCrypt.Net.BCrypt.Verify(input.SenhaAtual, usuario.SenhaHash))
        {
            throw new ArgumentException("Senha atual incorreta.");
        }

        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(input.NovaSenha);

        await _repository.Atualizar(usuario);
        await _repository.SaveChanges();
    }

    private static UsuarioResponseDTO MapearParaResponse(Usuario usuario)
    {
        return new UsuarioResponseDTO
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            CriadoEm = usuario.CriadoEm,
            Ativo = usuario.Ativo,
        };
    }
}

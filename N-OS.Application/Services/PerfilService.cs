using N_OS.Application.DTOs;
using N_OS.Application.Exceptions;
using N_OS.Application.Interfaces;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;

namespace N_OS.Application.Services;

public class PerfilService : IPerfilService
{
    private readonly IUsuarioRepository _repository;
    private readonly IRefreshTokenRepository _refreshTokens;

    public PerfilService(
        IUsuarioRepository repository,
        IRefreshTokenRepository refreshTokens)
    {
        _repository = repository;
        _refreshTokens = refreshTokens;
    }

    public async Task<UsuarioResponseDTO> Obter(int usuarioId)
    {
        var usuario = await BuscarOuFalhar(usuarioId);

        return UsuarioResponseDTO.De(usuario);
    }

    public async Task<UsuarioResponseDTO> Atualizar(
        int usuarioId,
        AtualizarPerfilDTO input)
    {
        var usuario = await BuscarOuFalhar(usuarioId);

        var email = AuthService.NormalizarEmail(input.Email);

        if (email != usuario.Email)
        {
            // o e-mail é o identificador de login: trocá-lo exige provar que
            // quem está na sessão sabe a senha
            await ConfirmarSenhaAtual(usuario, input.SenhaAtual);

            var emailEmUso = await _repository.BuscarPorEmail(email);

            if (emailEmUso != null && emailEmUso.Id != usuarioId)
            {
                throw new ArgumentException(
                    "Já existe um usuário cadastrado com este e-mail.");
            }

            usuario.Email = email;
        }

        usuario.Nome = input.Nome.Trim();

        await _repository.Atualizar(usuario);
        await _repository.SaveChanges();

        return UsuarioResponseDTO.De(usuario);
    }

    public async Task AlterarSenha(int usuarioId, AlterarSenhaDTO input)
    {
        var usuario = await BuscarOuFalhar(usuarioId);

        await ConfirmarSenhaAtual(usuario, input.SenhaAtual);

        if (input.NovaSenha == input.SenhaAtual)
        {
            throw new ArgumentException(
                "A nova senha deve ser diferente da senha atual.");
        }

        usuario.SenhaHash = SegurancaSenha.Gerar(input.NovaSenha);

        // invalida todos os tokens emitidos antes da troca (inclusive o atual)
        usuario.VersaoToken++;

        await _repository.Atualizar(usuario);
        await _repository.SaveChanges();

        // encerra também as renovações: nenhuma sessão antiga consegue voltar
        await _refreshTokens.RevogarTodosDoUsuario(usuario.Id, DateTime.UtcNow);
    }

    private async Task<Usuario> BuscarOuFalhar(int usuarioId)
    {
        return await _repository.BuscarPorId(usuarioId)
            ?? throw new NaoEncontradoException("Usuário não encontrado.");
    }

    // a senha atual errada conta para o mesmo bloqueio do login: sem isso,
    // quem tem uma sessão aberta poderia adivinhar a senha sem limite
    private async Task ConfirmarSenhaAtual(Usuario usuario, string? senhaAtual)
    {
        var agora = DateTime.UtcNow;

        if (usuario.EstaBloqueado(agora))
        {
            throw new ArgumentException(
                "Muitas tentativas incorretas. Tente novamente mais tarde.");
        }

        if (!string.IsNullOrEmpty(senhaAtual) &&
            SegurancaSenha.Verificar(senhaAtual, usuario.SenhaHash))
        {
            usuario.ZerarTentativasDeLogin();
            return;
        }

        usuario.RegistrarSenhaIncorreta(agora);

        await _repository.Atualizar(usuario);
        await _repository.SaveChanges();

        throw new ArgumentException("Senha atual incorreta.");
    }
}

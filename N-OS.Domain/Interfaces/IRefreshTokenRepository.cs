using N_OS.Domain.Entities;

namespace N_OS.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> BuscarPorHash(string tokenHash);

    Task Criar(RefreshToken token);

    /// <summary>
    /// Marca o token como usado só se ainda não foi usado nem revogado, numa única
    /// instrução atômica. Devolve false se outra requisição chegou primeiro.
    /// </summary>
    Task<bool> TentarMarcarComoUsado(int id, DateTime agora);

    /// <summary>Encerra todos os tokens da família.</summary>
    Task RevogarFamilia(Guid familiaId, DateTime agora);

    /// <summary>Encerra todas as sessões do usuário (troca de senha, por exemplo).</summary>
    Task RevogarTodosDoUsuario(int usuarioId, DateTime agora);

    /// <summary>Apaga tokens do usuário que expiraram antes do limite informado.</summary>
    Task RemoverExpirados(int usuarioId, DateTime expiradosAntesDe);

    Task SaveChanges();
}

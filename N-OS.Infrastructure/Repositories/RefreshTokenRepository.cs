using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;

namespace N_OS.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> BuscarPorHash(string tokenHash)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    }

    public Task Criar(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);

        return Task.CompletedTask;
    }

    public async Task<bool> TentarMarcarComoUsado(int id, DateTime agora)
    {
        var linhas = await _context.RefreshTokens
            .Where(t => t.Id == id && t.UsadoEm == null && t.RevogadoEm == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.UsadoEm, agora));

        return linhas == 1;
    }

    public async Task RevogarFamilia(Guid familiaId, DateTime agora)
    {
        await _context.RefreshTokens
            .Where(t => t.FamiliaId == familiaId && t.RevogadoEm == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevogadoEm, agora));
    }

    public async Task RevogarTodosDoUsuario(int usuarioId, DateTime agora)
    {
        await _context.RefreshTokens
            .Where(t => t.UsuarioId == usuarioId && t.RevogadoEm == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevogadoEm, agora));
    }

    public async Task RemoverExpirados(int usuarioId, DateTime expiradosAntesDe)
    {
        await _context.RefreshTokens
            .Where(t => t.UsuarioId == usuarioId && t.ExpiraEm < expiradosAntesDe)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}

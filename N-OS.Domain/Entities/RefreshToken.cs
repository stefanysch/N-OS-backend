namespace N_OS.Domain.Entities;

/// <summary>
/// Credencial de longa duração que renova o token de acesso. O valor em si
/// só existe no cookie do navegador; aqui fica apenas o hash (SHA-256).
/// Cada renovação consome o token e emite outro da mesma família.
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    /// <summary>SHA-256 (hex) do valor enviado ao navegador.</summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>Todos os tokens nascidos de um mesmo login compartilham a família.</summary>
    public Guid FamiliaId { get; set; }

    /// <summary>Quando o login que originou a família aconteceu (limite absoluto da sessão).</summary>
    public DateTime FamiliaCriadaEm { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime ExpiraEm { get; set; }

    /// <summary>Preenchido quando o token foi trocado por um novo (uso único).</summary>
    public DateTime? UsadoEm { get; set; }

    /// <summary>Preenchido quando a família foi encerrada (logout, reuso, troca de senha).</summary>
    public DateTime? RevogadoEm { get; set; }

    public string? IpCriacao { get; set; }
    public string? UserAgent { get; set; }
}

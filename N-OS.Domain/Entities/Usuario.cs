namespace N_OS.Domain.Entities;

public class Usuario
{
    private const int MaximoTentativasFalhas = 5;
    private static readonly TimeSpan DuracaoBloqueio = TimeSpan.FromMinutes(15);

    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    /// <summary>Falhas de senha seguidas desde o último acesso bem-sucedido.</summary>
    public int TentativasFalhasLogin { get; set; }

    /// <summary>Senha recusada até este instante (UTC) após excesso de falhas.</summary>
    public DateTime? BloqueadoAte { get; set; }

    /// <summary>
    /// Vai dentro do token. Incrementar (ex.: troca de senha) invalida
    /// todos os tokens já emitidos para este usuário.
    /// </summary>
    public int VersaoToken { get; set; }

    public bool EstaBloqueado(DateTime agora) =>
        BloqueadoAte is { } ate && ate > agora;

    /// <summary>Conta uma senha errada; na quinta seguida bloqueia a conta por 15 minutos.</summary>
    public void RegistrarSenhaIncorreta(DateTime agora)
    {
        TentativasFalhasLogin++;

        if (TentativasFalhasLogin >= MaximoTentativasFalhas)
        {
            BloqueadoAte = agora.Add(DuracaoBloqueio);
            TentativasFalhasLogin = 0;
        }
    }

    public void ZerarTentativasDeLogin()
    {
        TentativasFalhasLogin = 0;
        BloqueadoAte = null;
    }
}

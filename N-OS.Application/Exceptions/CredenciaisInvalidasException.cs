namespace N_OS.Application.Exceptions;

/// <summary>Login ou renovação de sessão recusados. Vira HTTP 401.</summary>
public class CredenciaisInvalidasException : Exception
{
    public CredenciaisInvalidasException(string mensagem, bool encerraSessao = true)
        : base(mensagem)
    {
        EncerraSessao = encerraSessao;
    }

    /// <summary>
    /// False quando a recusa é só uma corrida entre duas renovações do mesmo
    /// token: a outra requisição já gravou os cookies novos e não pode ter
    /// a sessão apagada por esta.
    /// </summary>
    public bool EncerraSessao { get; }
}

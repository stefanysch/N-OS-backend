namespace N_OS.Application.Exceptions;

/// <summary>Login recusado. Vira HTTP 401.</summary>
public class CredenciaisInvalidasException : Exception
{
    public CredenciaisInvalidasException(string mensagem) : base(mensagem)
    {
    }
}

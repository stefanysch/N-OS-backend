namespace N_OS.Application.Exceptions;

/// <summary>Recurso não existe. Vira HTTP 404.</summary>
public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}

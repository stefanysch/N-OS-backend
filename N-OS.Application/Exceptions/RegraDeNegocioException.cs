namespace N_OS.Application.Exceptions;

/// <summary>
/// Dados válidos, mas a operação conflita com o estado atual do sistema
/// (ex.: inativar cliente com veículos ativos). Vira HTTP 409.
/// </summary>
public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string mensagem) : base(mensagem)
    {
    }
}

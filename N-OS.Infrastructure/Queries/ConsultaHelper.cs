using System.Linq.Expressions;

namespace N_OS.Infrastructure.Queries;

internal static class ConsultaHelper
{
    public static IOrderedQueryable<T> Then<T, TKey>(
        IOrderedQueryable<T> ordenada,
        Expression<Func<T, TKey>> chave,
        bool descendente)
    {
        return descendente
            ? ordenada.ThenByDescending(chave)
            : ordenada.ThenBy(chave);
    }

    // padrão %texto% para ILike
    public static string PadraoLike(string texto)
    {
        var escapado = texto.Trim()
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");

        return $"%{escapado}%";
    }
}

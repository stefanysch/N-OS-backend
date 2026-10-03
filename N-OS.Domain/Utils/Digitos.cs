namespace N_OS.Domain.Utils;

public static class Digitos
{
    /// <summary>
    /// Mantém só os dígitos de 0 a 9 (remove máscara, espaços e letras).
    /// Nulo vira string vazia.
    /// </summary>
    public static string Extrair(string? valor)
    {
        return new string((valor ?? string.Empty).Where(char.IsAsciiDigit).ToArray());
    }
}

using N_OS.Domain.Enums;
using N_OS.Domain.Utils;

namespace N_OS.Domain.ValueObjects;

public class Documento : ValueObject
{
    public TipoDocumento Tipo { get; private set; }
    public string Numero { get; private set; }
    private Documento()
    {
        Numero = string.Empty;
    }
    public Documento(TipoDocumento tipo, string numero)
    {
        numero = Digitos.Extrair(numero);

        Validar(tipo, numero);

        Tipo = tipo;
        Numero = numero;
    }

    private static void Validar(TipoDocumento tipo, string numero)
    {
        if (tipo is not (TipoDocumento.CPF or TipoDocumento.CNPJ))
            throw new ArgumentException("Tipo de documento inválido.");

        if (!Valido(numero, tipo))
        {
            throw new ArgumentException(
                tipo == TipoDocumento.CPF ? "CPF inválido." : "CNPJ inválido.");
        }
    }

    /// <summary>
    /// Valida CPF (11 dígitos) ou CNPJ (14 dígitos), com dígitos verificadores.
    /// Recebe só dígitos. Sem <paramref name="tipo"/>, o tamanho decide qual é.
    /// Única implementação do algoritmo — usada também pelas validações de DTO.
    /// </summary>
    public static bool Valido(string numero, TipoDocumento? tipo = null)
    {
        if (!numero.All(char.IsAsciiDigit) || numero.Distinct().Count() == 1)
            return false;

        var ciclo = numero.Length switch
        {
            11 when tipo is null or TipoDocumento.CPF => 12,
            14 when tipo is null or TipoDocumento.CNPJ => 9,
            _ => 0,
        };

        if (ciclo == 0)
            return false;

        var baseDigitos = numero.Length - 2;

        return numero[baseDigitos] - '0' == DigitoVerificador(numero[..baseDigitos], ciclo)
            && numero[baseDigitos + 1] - '0' == DigitoVerificador(numero[..(baseDigitos + 1)], ciclo);
    }

    private static int DigitoVerificador(string digitos, int ciclo)
    {
        var soma = 0;

        for (var i = 0; i < digitos.Length; i++)
            soma += (digitos[digitos.Length - 1 - i] - '0') * (i % (ciclo - 1) + 2);

        var resto = soma % 11;

        return resto < 2 ? 0 : 11 - resto;
    }

    public override string ToString() => Numero;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Tipo;
        yield return Numero;
    }
}
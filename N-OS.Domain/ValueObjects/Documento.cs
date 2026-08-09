using System.Text.RegularExpressions;
using N_OS.Domain.Enums;

namespace N_OS.Domain.ValueObjects;

public class Documento
{
    public TipoDocumento Tipo { get; private set; }
    public string Numero { get; private set; }
    private Documento()
    {
        Numero = string.Empty;
    }
    public Documento(TipoDocumento tipo, string numero)
    {
        numero = Regex.Replace(numero, @"\D", "").Trim();

        Validar(tipo, numero);

        Tipo = tipo;
        Numero = numero;
    }

    private static void Validar(TipoDocumento tipo, string numero)
    {
        switch (tipo)
        {
            case TipoDocumento.CPF:
                if (numero.Length != 11 || !ValidarCpf(numero))
                    throw new ArgumentException("CPF inválido.");
                break;

            case TipoDocumento.CNPJ:
                if (numero.Length != 14 || !ValidarCnpj(numero))
                    throw new ArgumentException("CNPJ inválido.");
                break;

            default:
                throw new ArgumentException("Tipo de documento inválido.");
        }
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Distinct().Count() == 1)
            return false;

        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if ((cpf[9] - '0') != digito1)
            return false;

        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return (cpf[10] - '0') == digito2;
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Distinct().Count() == 1)
            return false;

        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += (cnpj[i] - '0') * multiplicador1[i];

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if ((cnpj[12] - '0') != digito1)
            return false;

        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += (cnpj[i] - '0') * multiplicador2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return (cnpj[13] - '0') == digito2;
    }

    public override string ToString() => Numero;
}
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace N_OS.Application.Validations;

public class CpfCnpjAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success;

        var documento = Regex.Replace(value.ToString()!, @"[.\-/]", "").Trim();

        if (documento.Length == 11 && ValidarCpf(documento))
            return ValidationResult.Success;

        if (documento.Length == 14 && ValidarCnpj(documento))
            return ValidationResult.Success;

        return new ValidationResult("Documento inválido. Informe um CPF ou CNPJ válido.");
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Distinct().Count() == 1) return false;

        var soma = 0;
        for (int i = 0; i < 9; i++)
            soma += int.Parse(cpf[i].ToString()) * (10 - i);

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;
        if (int.Parse(cpf[9].ToString()) != digito1) return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(cpf[i].ToString()) * (11 - i);

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;
        return int.Parse(cpf[10].ToString()) == digito2;
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Distinct().Count() == 1) return false;

        int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var soma = 0;
        for (int i = 0; i < 12; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicadores1[i];

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;
        if (int.Parse(cnpj[12].ToString()) != digito1) return false;

        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicadores2[i];

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;
        return int.Parse(cnpj[13].ToString()) == digito2;
    }
}
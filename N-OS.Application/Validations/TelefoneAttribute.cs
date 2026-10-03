using System.ComponentModel.DataAnnotations;
using N_OS.Domain.Utils;

namespace N_OS.Application.Validations;

/// <summary>
/// Valida telefone brasileiro (com ou sem máscara): 10 dígitos (fixo) ou
/// 11 (celular). Valor vazio é aceito: combine com [Required] quando o campo
/// for obrigatório.
/// </summary>
public class TelefoneAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success;

        var digitos = Digitos.Extrair(value.ToString());

        if (digitos.Length is 10 or 11)
            return ValidationResult.Success;

        return new ValidationResult("Telefone inválido. Informe DDD e número (10 ou 11 dígitos).");
    }
}

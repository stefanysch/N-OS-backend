using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace N_OS.Application.Validations;

/// <summary>
/// Valida placa de veículo (com ou sem hífen), no padrão antigo (ABC-1234)
/// ou Mercosul (ABC1D23). Valor vazio é aceito: combine com [Required]
/// quando o campo for obrigatório.
/// </summary>
public partial class PlacaAttribute : ValidationAttribute
{
    // 3 letras + 1 dígito + (letra ou dígito) + 2 dígitos
    [GeneratedRegex(@"^[A-Za-z]{3}-?\d[A-Za-z0-9]\d{2}$")]
    private static partial Regex PadraoPlaca();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success;

        if (PadraoPlaca().IsMatch(value.ToString()!.Trim()))
            return ValidationResult.Success;

        return new ValidationResult("Placa inválida. Use o formato ABC-1234 ou ABC1D23.");
    }
}

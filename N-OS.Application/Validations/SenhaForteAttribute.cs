using System.ComponentModel.DataAnnotations;
using System.Text;

namespace N_OS.Application.Validations;

/// <summary>
/// Política de senha para NOVAS senhas (cadastro e troca): 8 a 72 bytes
/// (o BCrypt ignora o que passa de 72), com ao menos uma letra e um número.
/// </summary>
public class SenhaForteAttribute : ValidationAttribute
{
    public const int Minimo = 8;
    public const int MaximoBytes = 72;

    public static string? Validar(string? senha)
    {
        if (string.IsNullOrEmpty(senha))
            return null; 

        if (senha.Length < Minimo)
            return $"A senha deve ter no mínimo {Minimo} caracteres.";

        if (Encoding.UTF8.GetByteCount(senha) > MaximoBytes)
            return $"A senha deve ter no máximo {MaximoBytes} caracteres.";

        if (!senha.Any(char.IsLetter) || !senha.Any(char.IsDigit))
            return "A senha deve conter ao menos uma letra e um número.";

        return null;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var erro = Validar(value as string);

        return erro is null ? ValidationResult.Success : new ValidationResult(erro);
    }
}

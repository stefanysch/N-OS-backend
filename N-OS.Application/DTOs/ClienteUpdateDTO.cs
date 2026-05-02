using System.ComponentModel.DataAnnotations;
using N_OS.Application.Validations;

namespace N_OS.Application.DTOs;

public class ClienteUpdateDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MinLength(3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [MinLength(10, ErrorMessage = "O telefone deve ter no mínimo 10 caracteres.")]
    [MaxLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [CpfCnpj]
    public string? Documento { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(200, ErrorMessage = "O e-mail deve ter no máximo 200 caracteres.")]
    public string? Email { get; set; }
}
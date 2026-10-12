using System.ComponentModel.DataAnnotations;
using N_OS.Application.Validations;

namespace N_OS.Application.DTOs;

public class RegistrarUsuarioDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MinLength(3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
    [MaxLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [SenhaForte]
    public string Senha { get; set; } = string.Empty;
}

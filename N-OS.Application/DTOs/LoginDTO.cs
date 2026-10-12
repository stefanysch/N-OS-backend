using System.ComponentModel.DataAnnotations;

namespace N_OS.Application.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
    [MaxLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MaxLength(200, ErrorMessage = "A senha deve ter no máximo 200 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

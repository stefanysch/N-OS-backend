using System.ComponentModel.DataAnnotations;
using N_OS.Domain.Enums;

namespace N_OS.Application.DTOs;

public class ClienteCreateDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MinLength(3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo do documento é obrigatório.")]
    public TipoDocumento TipoDocumento { get; set; }

    [Required(ErrorMessage = "O documento é obrigatório.")]
    [MinLength(11, ErrorMessage = "O documento deve ter no mínimo 11 caracteres.")]
    [MaxLength(14, ErrorMessage = "O documento deve ter no máximo 14 caracteres.")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [MinLength(10, ErrorMessage = "O telefone deve ter no mínimo 10 caracteres.")]
    [MaxLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "O email informado é inválido.")]
    [MaxLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres.")]
    public string? Email { get; set; }  

    [Required]
    [MaxLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres.")]
    public string Cep { get; set; } = string.Empty;

    [Required]
    [MaxLength(150, ErrorMessage = "O logradouro deve ter no máximo 150 caracteres.")]
    public string Logradouro { get; set; } = string.Empty;

    [Required]
    [MaxLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres.")]
    public string Numero { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres.")]
    public string? Complemento { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "O bairro deve ter no máximo 50 caracteres.")]
    public string Bairro { get; set; } = string.Empty;

    [Required]
    [MaxLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [Required]
    [StringLength(2, ErrorMessage = "O estado deve ter no máximo 2 caracteres.")]
    public string Estado { get; set; } = string.Empty;
}
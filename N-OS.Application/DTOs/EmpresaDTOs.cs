using System.ComponentModel.DataAnnotations;

namespace N_OS.Application.DTOs;

public class EmpresaResponseDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Documento { get; set; }

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string? Endereco { get; set; }

    public bool Configurada { get; set; }
}

public class EmpresaUpdateDTO
{
    [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "O documento deve ter no máximo 20 caracteres.")]
    public string? Documento { get; set; }

    [MaxLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string? Telefone { get; set; }

    [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
    [MaxLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string? Email { get; set; }

    [MaxLength(250, ErrorMessage = "O endereço deve ter no máximo 250 caracteres.")]
    public string? Endereco { get; set; }
}

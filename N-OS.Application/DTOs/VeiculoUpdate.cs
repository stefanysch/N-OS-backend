using System.ComponentModel.DataAnnotations;

namespace N_OS.Application.DTOs;

public class VeiculoUpdateDTO
{
    [Required(ErrorMessage = "O cliente é obrigatório.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "A placa é obrigatória.")]
    [MinLength(7, ErrorMessage = "A placa deve ter no mínimo 7 caracteres.")]
    [MaxLength(10, ErrorMessage = "A placa deve ter no máximo 10 caracteres.")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "A marca é obrigatória.")]
    [MinLength(3, ErrorMessage = "A marca deve ter no mínimo 3 caracteres.")]
    [MaxLength(100, ErrorMessage = "A marca deve ter no máximo 100 caracteres.")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    [MinLength(3, ErrorMessage = "O modelo deve ter no mínimo 3 caracteres.")]
    [MaxLength(100, ErrorMessage = "O modelo deve ter no máximo 100 caracteres.")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ano é obrigatório.")]
    [Range(1886, int.MaxValue, ErrorMessage = "O ano deve ser um valor válido.")]
    public int Ano { get; set; }

    [MinLength(3, ErrorMessage = "A cor deve ter no mínimo 3 caracteres.")]
    [MaxLength(50, ErrorMessage = "A cor deve ter no máximo 50 caracteres.")]
    public string? Cor { get; set; }

    [MinLength(3, ErrorMessage = "O chassi deve ter no mínimo 3 caracteres.")]
    [MaxLength(30, ErrorMessage = "O chassi deve ter no máximo 30 caracteres.")]
    public string? Chassi { get; set; }
}
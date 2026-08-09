using System.ComponentModel.DataAnnotations;

namespace N_OS.Application.DTOs;
public class OrdemDeServicoUpdateDTO
{
    [MinLength(3, ErrorMessage = "A descrição deve ter no mínimo 3 caracteres.")]
    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string DescricaoProblema { get; set; } = string.Empty;

    [MinLength(3, ErrorMessage = "A observação deve ter no mínimo 3 caracteres.")]
    [MaxLength(500, ErrorMessage = "A observação deve ter no máximo 500 caracteres.")]
    public string? Observacoes { get; set; }

    public List<ItemOSCreateDTO> Itens { get; set; } = [];
}


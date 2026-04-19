using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace N_OS.Domain.Entities;

public class Cliente
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;
}
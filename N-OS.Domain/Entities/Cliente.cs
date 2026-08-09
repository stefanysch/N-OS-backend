using N_OS.Domain.ValueObjects;

namespace N_OS.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public Documento Documento { get; set; } = null!;

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Endereco Endereco { get; set; } = null!;

    public ICollection<Veiculo> Veiculos { get; set; } = [];

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public bool Ativo { get; set; } = true;
}
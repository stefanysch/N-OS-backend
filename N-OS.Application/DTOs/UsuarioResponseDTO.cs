using N_OS.Domain.Entities;

namespace N_OS.Application.DTOs;

public class UsuarioResponseDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public bool Ativo { get; set; }

    public static UsuarioResponseDTO De(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        CriadoEm = usuario.CriadoEm,
        Ativo = usuario.Ativo,
    };
}

namespace N_OS.Application.DTOs;

/// <summary>
/// Resultado interno de login/renovação. A camada Application não conhece HTTP:
/// quem grava os cookies é o controller.
/// </summary>
public class SessaoResultado
{
    public string TokenAcesso { get; set; } = string.Empty;
    public DateTime TokenAcessoExpiraEm { get; set; }

    /// <summary>Valor em texto puro do refresh token. Só existe aqui e no cookie.</summary>
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiraEm { get; set; }

    public LoginResponseDTO Usuario { get; set; } = new();
}

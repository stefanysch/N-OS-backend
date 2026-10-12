namespace N_OS.Application.Services;

/// <summary>Hash e verificação de senha (BCrypt) num só lugar.</summary>
public static class SegurancaSenha
{
    // custo do BCrypt: cada +1 dobra o tempo de hash. 12 é um bom equilíbrio hoje.
    public const int CustoBcrypt = 12;

    // hash de uma senha qualquer, usado para gastar o mesmo tempo quando o
    // e-mail não existe (evita descobrir e-mails cadastrados pelo tempo da resposta)
    private static readonly string HashFalso =
        BCrypt.Net.BCrypt.HashPassword("senha-falsa-para-igualar-tempo", CustoBcrypt);

    public static string Gerar(string senha) =>
        BCrypt.Net.BCrypt.HashPassword(senha, CustoBcrypt);

    public static bool Verificar(string senha, string hash) =>
        BCrypt.Net.BCrypt.Verify(senha, hash);

    public static void VerificarFalso(string senha) =>
        BCrypt.Net.BCrypt.Verify(senha, HashFalso);

    public static bool PrecisaRehash(string hash) =>
        BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, CustoBcrypt);
}

using N_OS.Application.DTOs;

namespace N_OS.API.Seguranca;

/// <summary>
/// Grava e apaga os cookies de sessão. Os dois são HttpOnly (JavaScript não lê),
/// SameSite=Strict (o navegador não os envia em requisições vindas de outro site)
/// e Secure fora de desenvolvimento (só trafegam por HTTPS).
/// </summary>
public class CookiesSessao
{
    public const string Acesso = "nos_at";
    public const string Refresh = "nos_rt";

    // o cookie de acesso vai em toda chamada da API; o de refresh só nas rotas de auth
    private const string CaminhoAcesso = "/api";
    private const string CaminhoRefresh = "/api/auth";

    private readonly bool _seguro;

    public CookiesSessao(IWebHostEnvironment ambiente)
    {
        _seguro = !ambiente.IsDevelopment();
    }

    public void Definir(HttpResponse resposta, SessaoResultado sessao)
    {
        resposta.Cookies.Append(
            Acesso,
            sessao.TokenAcesso,
            Opcoes(CaminhoAcesso, sessao.TokenAcessoExpiraEm));

        resposta.Cookies.Append(
            Refresh,
            sessao.RefreshToken,
            Opcoes(CaminhoRefresh, sessao.RefreshTokenExpiraEm));
    }

    public void Limpar(HttpResponse resposta)
    {
        resposta.Cookies.Delete(Acesso, Opcoes(CaminhoAcesso, null));
        resposta.Cookies.Delete(Refresh, Opcoes(CaminhoRefresh, null));
    }

    private CookieOptions Opcoes(string caminho, DateTime? expiraEm) => new()
    {
        HttpOnly = true,
        Secure = _seguro,
        SameSite = SameSiteMode.Strict,
        Path = caminho,
        IsEssential = true,
        Expires = expiraEm is null
            ? null
            : new DateTimeOffset(DateTime.SpecifyKind(expiraEm.Value, DateTimeKind.Utc)),
    };
}

namespace N_OS.API.Seguranca;

/// <summary>
/// Proteção contra CSRF para requisições que alteram dados (POST, PUT, PATCH, DELETE).
/// Como a autenticação vai em cookie, o navegador a envia sozinho; então só aceitamos
/// a requisição se ela partiu do nosso front:
/// - com cabeçalho Origin: precisa ser uma das origens liberadas (ou a própria API,
///   caso do Swagger em desenvolvimento);
/// - sem Origin: só passa se a requisição não traz cookie de sessão.
/// Complementa o SameSite=Strict dos cookies.
/// </summary>
public class ValidacaoDeOrigemMiddleware
{
    private readonly RequestDelegate _proximo;
    private readonly HashSet<string> _origensPermitidas;

    public ValidacaoDeOrigemMiddleware(RequestDelegate proximo, IEnumerable<string> origensPermitidas)
    {
        _proximo = proximo;
        _origensPermitidas = new HashSet<string>(
            origensPermitidas.Select(o => o.TrimEnd('/')),
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        if (!ExigeValidacao(contexto.Request) || OrigemAceita(contexto.Request))
        {
            await _proximo(contexto);
            return;
        }

        contexto.Response.StatusCode = StatusCodes.Status403Forbidden;

        await contexto.Response.WriteAsJsonAsync(new
        {
            status = StatusCodes.Status403Forbidden,
            title = "Origem não permitida",
            mensagem = "Requisição recusada: origem não permitida.",
        });
    }

    private static bool ExigeValidacao(HttpRequest requisicao) =>
        requisicao.Path.StartsWithSegments("/api") &&
        !HttpMethods.IsGet(requisicao.Method) &&
        !HttpMethods.IsHead(requisicao.Method) &&
        !HttpMethods.IsOptions(requisicao.Method) &&
        !HttpMethods.IsTrace(requisicao.Method);

    private bool OrigemAceita(HttpRequest requisicao)
    {
        var origem = requisicao.Headers.Origin.ToString().TrimEnd('/');

        if (string.IsNullOrEmpty(origem))
        {
            return !requisicao.Cookies.ContainsKey(CookiesSessao.Acesso) &&
                   !requisicao.Cookies.ContainsKey(CookiesSessao.Refresh);
        }

        var propriaOrigem = $"{requisicao.Scheme}://{requisicao.Host}";

        return _origensPermitidas.Contains(origem) ||
               string.Equals(origem, propriaOrigem, StringComparison.OrdinalIgnoreCase);
    }
}

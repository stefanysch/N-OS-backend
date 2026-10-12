using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using N_OS.API.Seguranca;
using N_OS.Application.DTOs;
using N_OS.Application.Exceptions;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

/// <summary>
/// Login, renovação e encerramento de sessão (públicos) e cadastro de usuários
/// (exige estar logado). A sessão vive em cookies HttpOnly; o corpo das respostas
/// nunca traz token.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly CookiesSessao _cookies;

    public AuthController(IAuthService authService, CookiesSessao cookies)
    {
        _authService = authService;
        _cookies = cookies;
    }

    /// <summary>Cadastra um novo usuário. Exige estar logado.</summary>
    /// <response code="200">Usuário criado.</response>
    /// <response code="400">Campos inválidos ou e-mail já cadastrado.</response>
    /// <response code="401">Sem sessão válida.</response>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarUsuarioDTO input)
    {
        var usuario = await _authService.Registrar(input);

        return Ok(usuario);
    }

    /// <summary>Autentica o usuário e abre a sessão.</summary>
    /// <remarks>
    /// Grava dois cookies HttpOnly: `nos_at` (acesso, curta duração) e `nos_rt`
    /// (renovação). O corpo traz só os dados do usuário.
    /// </remarks>
    /// <response code="200">Login realizado.</response>
    /// <response code="400">Campos inválidos.</response>
    /// <response code="401">E-mail ou senha incorretos, usuário inativo ou conta bloqueada por excesso de tentativas.</response>
    /// <response code="429">Muitas requisições deste IP em pouco tempo.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDTO input)
    {
        var sessao = await _authService.Login(input, ObterIp(), ObterUserAgent());

        _cookies.Definir(Response, sessao);

        return Ok(sessao.Usuario);
    }

    /// <summary>Renova a sessão usando o cookie de renovação.</summary>
    /// <remarks>
    /// Troca o refresh token (uso único) por um novo par de cookies. Se a sessão
    /// não puder ser renovada, os cookies são apagados e a resposta é 401.
    /// </remarks>
    /// <response code="200">Sessão renovada.</response>
    /// <response code="401">Sessão expirada, encerrada ou inválida.</response>
    /// <response code="429">Muitas requisições deste IP em pouco tempo.</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("refresh")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Renovar()
    {
        try
        {
            var sessao = await _authService.Renovar(
                Request.Cookies[CookiesSessao.Refresh] ?? string.Empty,
                ObterIp(),
                ObterUserAgent());

            _cookies.Definir(Response, sessao);

            return Ok(sessao.Usuario);
        }
        catch (CredenciaisInvalidasException erro)
        {
            // a resposta é montada aqui (e não pelo handler global) para o
            // Set-Cookie que apaga os cookies não ser descartado. Numa corrida entre
            // duas renovações os cookies novos já foram gravados pela outra: não apagar.
            if (erro.EncerraSessao)
            {
                _cookies.Limpar(Response);
            }

            return StatusCode(StatusCodes.Status401Unauthorized, new
            {
                status = StatusCodes.Status401Unauthorized,
                title = "Não autenticado",
                mensagem = erro.Message,
            });
        }
    }

    /// <summary>Encerra a sessão e apaga os cookies.</summary>
    /// <remarks>Pública de propósito: precisa funcionar mesmo com o token de acesso vencido.</remarks>
    /// <response code="200">Sessão encerrada.</response>
    [HttpPost("logout")]
    [AllowAnonymous]
    [EnableRateLimiting("refresh")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> Sair()
    {
        await _authService.Sair(Request.Cookies[CookiesSessao.Refresh]);

        _cookies.Limpar(Response);

        return Ok(new MensagemResponseDTO("Sessão encerrada."));
    }

    private string? ObterIp() =>
        HttpContext.Connection.RemoteIpAddress?.ToString();

    private string? ObterUserAgent() =>
        Request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua : null;
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

/// <summary>Cadastro de usuários e login. Rotas públicas, não exigem token.</summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Cadastra um novo usuário.</summary>
    /// <response code="200">Usuário criado.</response>
    /// <response code="400">Campos inválidos ou e-mail já cadastrado.</response>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarUsuarioDTO input)
    {
        var usuario = await _authService.Registrar(input);

        return Ok(usuario);
    }

    /// <summary>Autentica o usuário e devolve o token JWT.</summary>
    /// <remarks>
    /// Envie o token no header `Authorization: Bearer {token}` nas demais rotas.
    /// O token expira em `expiraEm`, sem tolerância de relógio.
    /// </remarks>
    /// <response code="200">Login realizado.</response>
    /// <response code="400">Campos inválidos.</response>
    /// <response code="401">E-mail ou senha incorretos, ou usuário inativo.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDTO input)
    {
        var resposta = await _authService.Login(input);

        return Ok(resposta);
    }
}

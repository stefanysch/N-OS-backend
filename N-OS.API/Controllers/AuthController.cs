using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarUsuarioDTO input)
    {
        try
        {
            var usuario = await _authService.Registrar(input);

            return Ok(usuario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDTO input)
    {
        try
        {
            var resposta = await _authService.Login(input);

            return Ok(resposta);
        }
        catch (ArgumentException ex)
        {
            return Unauthorized(new
            {
                mensagem = ex.Message
            });
        }
    }
}

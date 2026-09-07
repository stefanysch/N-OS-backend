using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

[ApiController]
[Route("api/perfil")]
public class PerfilController : ControllerBase
{
    private readonly IPerfilService _perfilService;

    public PerfilController(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    [HttpGet]
    public async Task<IActionResult> Obter()
    {
        try
        {
            var perfil = await _perfilService.Obter(ObterUsuarioId());

            return Ok(perfil);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Atualizar(
        [FromBody] AtualizarPerfilDTO input)
    {
        try
        {
            var perfil = await _perfilService.Atualizar(
                ObterUsuarioId(),
                input);

            return Ok(perfil);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("senha")]
    public async Task<IActionResult> AlterarSenha(
        [FromBody] AlterarSenhaDTO input)
    {
        try
        {
            await _perfilService.AlterarSenha(ObterUsuarioId(), input);

            return Ok(new { mensagem = "Senha alterada com sucesso." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private int ObterUsuarioId()
    {
        var valor =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return int.Parse(valor!);
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using N_OS.Application.DTOs;
using N_OS.Application.Interfaces;

namespace N_OS.API.Controllers;

/// <summary>Perfil do usuário autenticado (sempre o dono do token).</summary>
[ApiController]
[Route("api/perfil")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class PerfilController : ControllerBase
{
    private readonly IPerfilService _perfilService;

    public PerfilController(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    /// <summary>Retorna os dados do usuário logado.</summary>
    /// <response code="200">Dados do usuário.</response>
    /// <response code="404">Usuário do token não existe mais.</response>
    [HttpGet]
    [ProducesResponseType(typeof(UsuarioResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter()
    {
        var perfil = await _perfilService.Obter(ObterUsuarioId());

        return Ok(perfil);
    }

    /// <summary>Atualiza nome e e-mail do usuário logado.</summary>
    /// <response code="200">Dados atualizados.</response>
    /// <response code="400">Campos inválidos ou e-mail já usado por outro usuário.</response>
    /// <response code="404">Usuário do token não existe mais.</response>
    [HttpPut]
    [ProducesResponseType(typeof(UsuarioResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(
        [FromBody] AtualizarPerfilDTO input)
    {
        var perfil = await _perfilService.Atualizar(
            ObterUsuarioId(),
            input);

        return Ok(perfil);
    }

    /// <summary>Altera a senha do usuário logado.</summary>
    /// <response code="200">Senha alterada.</response>
    /// <response code="400">Campos inválidos ou senha atual incorreta.</response>
    /// <response code="404">Usuário do token não existe mais.</response>
    [HttpPut("senha")]
    [ProducesResponseType(typeof(MensagemResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarSenha(
        [FromBody] AlterarSenhaDTO input)
    {
        await _perfilService.AlterarSenha(ObterUsuarioId(), input);

        return Ok(new MensagemResponseDTO("Senha alterada com sucesso."));
    }

    private int ObterUsuarioId()
    {
        var valor =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return int.Parse(valor!);
    }
}

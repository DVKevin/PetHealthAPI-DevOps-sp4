using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Interfaces;

namespace PetHealthAPI.Controllers;

/// <summary>
/// Cadastro de usuários e login (emissão do token JWT).
/// O cadastro público cria perfil "Usuario"; o "Admin" é criado automaticamente na inicialização.
/// </summary>
[Route("api/[controller]")]
public class AutenticacaoController : ApiControllerBase
{
    private readonly IAutenticacaoServico _autenticacaoServico;

    public AutenticacaoController(IAutenticacaoServico autenticacaoServico)
    {
        _autenticacaoServico = autenticacaoServico;
    }

    /// <summary>Cadastra um novo usuário (perfil Usuario). Senha com no mínimo 6 caracteres.</summary>
    [HttpPost("registrar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Registrar([FromBody] RegistrarUsuarioRequest request)
    {
        var usuario = await _autenticacaoServico.RegistrarAsync(request);
        return StatusCode(StatusCodes.Status201Created, usuario);
    }

    /// <summary>Autentica o usuário e retorna o token JWT (use no botão Authorize do Swagger).</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
    {
        var token = await _autenticacaoServico.LoginAsync(request);
        return Ok(token);
    }

    /// <summary>Mostra os dados do usuário dono do token (útil para conferir o login e o perfil).</summary>
    [HttpGet("eu")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Eu()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"),
            nome = User.Identity?.Name,
            email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email"),
            perfil = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}

using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Services;
using ErrorOr;

namespace UserService.API.Controllers;

/// <summary>
/// Controlador para autenticação e registro de usuários.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza o login do usuário e retorna um token JWT.
    /// </summary>
    /// <param name="loginDto">Dados de login do usuário (email e senha).</param>
    /// <returns>Retorna o token JWT e informações do usuário autenticado.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(loginDto);

        return result.Match<ActionResult>(
            sucesso => Ok(sucesso),
            erros =>
            {
                var erro = erros.First();
                return erro.Type switch
                {
                    ErrorType.NotFound => Unauthorized(new { message = erro.Description }),
                    ErrorType.Validation => BadRequest(new { message = erro.Description }),
                    _ => StatusCode(500, new { message = erro.Description })
                };
            }
        );
    }

    /// <summary>
    /// Realiza o registro de um novo usuário e retorna um token JWT.
    /// </summary>
    /// <param name="registerDto">Dados do usuário para registro.</param>
    /// <returns>Retorna o usuário autenticado e o token JWT.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(registerDto);

        return result.Match<ActionResult>(
            sucesso => CreatedAtAction(nameof(Login), new { }, sucesso),
            erros =>
            {
                var erro = erros.First();
                return erro.Type switch
                {
                    ErrorType.Conflict => Conflict(new { message = erro.Description }),
                    ErrorType.Validation => BadRequest(new { message = erro.Description }),
                    _ => StatusCode(500, new { message = erro.Description })
                };
            }
        );
    }

    /// <summary>
    /// Gera um novo token JWT a partir de um refresh token válido.
    /// </summary>
    /// <param name="refreshTokenDto">Objeto contendo o refresh token válido.</param>
    /// <returns>Retorna um novo token JWT e refresh token.</returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            return Ok(response);
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, "Refresh token functionality not implemented yet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, "An error occurred while refreshing token");
        }
    }
}

public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
} 
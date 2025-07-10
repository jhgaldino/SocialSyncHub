using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Services;

namespace UserService.API.Controllers;

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

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(loginDto);
            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Failed login attempt for email {Email}", loginDto.Email);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", loginDto.Email);
            return StatusCode(500, "An error occurred during login");
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAsync(registerDto);
            _logger.LogInformation("User {Email} registered successfully", registerDto.Email);
            
            return CreatedAtAction(nameof(Login), new { }, response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed registration attempt for email {Email}: {Message}", registerDto.Email, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email {Email}", registerDto.Email);
            return StatusCode(500, "An error occurred during registration");
        }
    }

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
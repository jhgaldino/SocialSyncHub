using UserService.Application.DTOs;

namespace UserService.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
} 
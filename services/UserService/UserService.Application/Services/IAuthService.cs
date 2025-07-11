using UserService.Application.DTOs;
using ErrorOr;

namespace UserService.Application.Services;

public interface IAuthService
{
    Task<ErrorOr<AuthResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ErrorOr<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
} 
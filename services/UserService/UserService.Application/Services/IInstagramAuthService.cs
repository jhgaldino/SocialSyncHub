using UserService.Application.DTOs;

namespace UserService.Application.Services;

public interface IInstagramAuthService
{
    string GetAuthorizationUrl(Guid userId);
    Task<InstagramTokenResponseDto> ExchangeCodeForTokenAsync(string code, string redirectUri);
}

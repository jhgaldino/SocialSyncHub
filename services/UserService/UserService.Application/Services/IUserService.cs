using UserService.Application.DTOs;
using ErrorOr;

namespace UserService.Application.Services;

public interface IUserService
{
    Task<ErrorOr<UserDto>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<UserDto>>> GetAllAsync();
    Task<ErrorOr<UserDto>> CreateAsync(CreateUserDto createUserDto);
    Task<bool> ExistsAsync(Guid id);

    // Social account management
    Task<List<SocialAccountDto>> GetSocialAccountsAsync(Guid userId);
    Task<ErrorOr<SocialAccountDto>> ConnectSocialAccountAsync(Guid userId, ConnectSocialAccountDto dto);
    Task<ErrorOr<bool>> DisconnectSocialAccountAsync(Guid userId, SocialNetworkTypeDto networkType);
} 
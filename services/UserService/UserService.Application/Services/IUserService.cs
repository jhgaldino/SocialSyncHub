using UserService.Application.DTOs;

namespace UserService.Application.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto createUserDto);
    Task<bool> ExistsAsync(Guid id);
} 
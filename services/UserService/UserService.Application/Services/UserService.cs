using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        // Verificar se o email já existe
        if (await _userRepository.ExistsByEmailAsync(createUserDto.Email))
        {
            throw new InvalidOperationException($"User with email '{createUserDto.Email}' already exists.");
        }

        var user = new User(createUserDto.Name, createUserDto.Email, "");
        await _userRepository.AddAsync(user);

        return MapToDto(user);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _userRepository.ExistsAsync(id);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
} 
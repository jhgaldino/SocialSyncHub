using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using AutoMapper;
using ErrorOr;

namespace UserService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return Error.NotFound(description: $"Usuário com ID {id} não encontrado.");
        }
        return _mapper.Map<UserDto>(user);
    }

    public async Task<ErrorOr<List<UserDto>>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = users.Select(u => _mapper.Map<UserDto>(u)).ToList();
        return userDtos;
    }

    public async Task<ErrorOr<UserDto>> CreateAsync(CreateUserDto createUserDto)
    {
        if (await _userRepository.ExistsByEmailAsync(createUserDto.Email))
        {
            return Error.Conflict(description: $"Usuário com email '{createUserDto.Email}' já existe.");
        }
        var user = _mapper.Map<User>(createUserDto);
        await _userRepository.AddAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _userRepository.ExistsAsync(id);
    }
} 
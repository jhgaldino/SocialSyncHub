using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using AutoMapper;
using ErrorOr;

namespace UserService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ISocialAccountRepository _socialAccountRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, ISocialAccountRepository socialAccountRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _socialAccountRepository = socialAccountRepository;
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

    public async Task<List<SocialAccountDto>> GetSocialAccountsAsync(Guid userId)
    {
        var accounts = await _socialAccountRepository.GetByUserIdAsync(userId);
        return accounts.Select(a => _mapper.Map<SocialAccountDto>(a)).ToList();
    }

    public async Task<ErrorOr<SocialAccountDto>> ConnectSocialAccountAsync(Guid userId, ConnectSocialAccountDto dto)
    {
        // Verifica se o usuário existe
        if (!await _userRepository.ExistsAsync(userId))
        {
            return Error.NotFound(description: "Usuário não encontrado.");
        }

        // Verifica se já existe uma conta conectada para esta rede social
        var existing = await _socialAccountRepository.GetByUserAndNetworkAsync(userId, (SocialNetworkType)dto.NetworkType);
        if (existing != null)
        {
            return Error.Conflict(description: $"Conta já conectada para {dto.NetworkType}");
        }

        // Cria nova conta social
        var account = new SocialAccount
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NetworkType = (SocialNetworkType)dto.NetworkType,
            AccessToken = dto.AccessToken,
            RefreshToken = dto.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(60) // Token expira em 60 dias por padrão
        };

        await _socialAccountRepository.AddAsync(account);

        return _mapper.Map<SocialAccountDto>(account);
    }

    public async Task<ErrorOr<bool>> DisconnectSocialAccountAsync(Guid userId, SocialNetworkTypeDto networkType)
    {
        var account = await _socialAccountRepository.GetByUserAndNetworkAsync(userId, (SocialNetworkType)networkType);
        if (account == null)
        {
            return Error.NotFound(description: $"Conta não encontrada para {networkType}");
        }

        await _socialAccountRepository.RemoveAsync(account.Id);
        return true;
    }
}
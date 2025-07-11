using FluentAssertions;
using Moq;
using UserService.Application.DTOs;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using Xunit;
using ErrorOr;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ISocialAccountRepository> _mockSocialAccountRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserService.Application.Services.UserService _userService;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockSocialAccountRepository = new Mock<ISocialAccountRepository>();
        _mockMapper = new Mock<IMapper>();
        _userService = new UserService.Application.Services.UserService(_mockRepository.Object, _mockSocialAccountRepository.Object, _mockMapper.Object);

        // Configurar o mock do IMapper para retornar UserDto válido
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns((User u) => u == null ? new UserDto() : new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        });
        _mockMapper.Setup(m => m.Map<List<UserDto>>(It.IsAny<IEnumerable<User>>())).Returns((IEnumerable<User> users) => users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        }).ToList());
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<object>())).Returns((object o) =>
        {
            if (o is User u)
            {
                return new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                };
            }
            return new UserDto
            {
                Id = Guid.NewGuid(),
                Name = "New User",
                Email = "newuser@example.com",
                CreatedAt = DateTime.UtcNow
            };
        });
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("Test User", "test@example.com", "hashedPassword")
        {
            Id = userId,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(userId);
        result.Value.Name.Should().Be("Test User");
        result.Value.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CreateAsync_WhenValidUser_ShouldCreateAndReturnUser()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "newuser@example.com"
        };

        _mockRepository.Setup(r => r.ExistsByEmailAsync(createUserDto.Email))
            .ReturnsAsync(false);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.CreateAsync(createUserDto);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("New User");
        result.Value.Email.Should().Be("newuser@example.com");
        result.Value.Id.Should().NotBeEmpty();
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ShouldReturnConflictError()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "existing@example.com"
        };

        _mockRepository.Setup(r => r.ExistsByEmailAsync(createUserDto.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.CreateAsync(createUserDto);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new("User 1", "user1@example.com", "hash1") { Id = Guid.NewGuid() },
            new("User 2", "user2@example.com", "hash2") { Id = Guid.NewGuid() }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(u => u.Name == "User 1");
        result.Value.Should().Contain(u => u.Name == "User 2");
    }

    [Fact]
    public async Task ExistsAsync_WhenUserExists_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.ExistsAsync(userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenUserDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.ExistsAsync(userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_Should_UseCache_When_UserIsCached()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto
        {
            Id = userId,
            Name = "User Cache",
            Email = "cache@example.com",
            CreatedAt = DateTime.UtcNow
        };
        var cacheKey = $"user:{userId}";
        var cache = new Mock<IDistributedCache>();
        cache.Setup(c => c.GetAsync(cacheKey, default)).ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(userDto)));

        // Simula controller recebendo cache
        var controller = new UserService.API.Controllers.UserController(_userService, null, null, cache.Object);
        var result = await controller.GetById(userId);

        var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userDto.Id, returnedUser.Id);
        Assert.Equal(userDto.Name, returnedUser.Name);
        Assert.Equal(userDto.Email, returnedUser.Email);
    }

    [Fact]
    public async Task GetSocialAccountsAsync_ShouldReturnAccounts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accounts = new List<SocialAccount>
        {
            new SocialAccount { Id = Guid.NewGuid(), UserId = userId, NetworkType = SocialNetworkType.Instagram, Username = "insta_user" },
            new SocialAccount { Id = Guid.NewGuid(), UserId = userId, NetworkType = SocialNetworkType.TikTok, Username = "tiktok_user" }
        };
        _mockSocialAccountRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(accounts);
        _mockMapper.Setup(m => m.Map<SocialAccountDto>(It.IsAny<SocialAccount>())).Returns<SocialAccount>(a => new SocialAccountDto
        {
            Id = a.Id,
            NetworkType = (SocialNetworkTypeDto)a.NetworkType,
            Username = a.Username ?? string.Empty
        });

        // Act
        var result = await _userService.GetSocialAccountsAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result[0].Username.Should().Be("insta_user");
        result[1].NetworkType.Should().Be(SocialNetworkTypeDto.TikTok);
    }

    [Fact]
    public async Task ConnectSocialAccountAsync_WhenNotExists_ShouldConnect()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new ConnectSocialAccountDto { NetworkType = SocialNetworkTypeDto.X, AccessToken = "token" };
        _mockRepository.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(true);
        _mockSocialAccountRepository.Setup(r => r.GetByUserAndNetworkAsync(userId, SocialNetworkType.X)).ReturnsAsync((SocialAccount)null);
        _mockSocialAccountRepository.Setup(r => r.AddAsync(It.IsAny<SocialAccount>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<SocialAccountDto>(It.IsAny<SocialAccount>())).Returns<SocialAccount>(a => new SocialAccountDto
        {
            Id = a.Id,
            NetworkType = (SocialNetworkTypeDto)a.NetworkType,
            Username = a.Username ?? string.Empty
        });

        // Act
        var result = await _userService.ConnectSocialAccountAsync(userId, dto);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.NetworkType.Should().Be(SocialNetworkTypeDto.X);
    }

    [Fact]
    public async Task ConnectSocialAccountAsync_WhenAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new ConnectSocialAccountDto { NetworkType = SocialNetworkTypeDto.Instagram, AccessToken = "token" };
        _mockRepository.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(true);
        _mockSocialAccountRepository.Setup(r => r.GetByUserAndNetworkAsync(userId, SocialNetworkType.Instagram)).ReturnsAsync(new SocialAccount());

        // Act
        var result = await _userService.ConnectSocialAccountAsync(userId, dto);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task DisconnectSocialAccountAsync_WhenExists_ShouldDisconnect()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var account = new SocialAccount { Id = Guid.NewGuid(), UserId = userId, NetworkType = SocialNetworkType.TikTok };
        _mockSocialAccountRepository.Setup(r => r.GetByUserAndNetworkAsync(userId, SocialNetworkType.TikTok)).ReturnsAsync(account);
        _mockSocialAccountRepository.Setup(r => r.RemoveAsync(account.Id)).Returns(Task.CompletedTask);

        // Act
        var result = await _userService.DisconnectSocialAccountAsync(userId, SocialNetworkTypeDto.TikTok);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task DisconnectSocialAccountAsync_WhenNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockSocialAccountRepository.Setup(r => r.GetByUserAndNetworkAsync(userId, SocialNetworkType.TikTok)).ReturnsAsync((SocialAccount)null);

        // Act
        var result = await _userService.DisconnectSocialAccountAsync(userId, SocialNetworkTypeDto.TikTok);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }
}
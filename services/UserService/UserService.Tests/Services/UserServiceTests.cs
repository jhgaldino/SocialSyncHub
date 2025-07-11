using FluentAssertions;
using Moq;
using UserService.Application.DTOs;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using Xunit;
using ErrorOr;
using AutoMapper;

namespace UserService.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserService.Application.Services.UserService _userService;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _userService = new UserService.Application.Services.UserService(_mockRepository.Object, _mockMapper.Object);

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
} 
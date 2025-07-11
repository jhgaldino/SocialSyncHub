using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using UserService.Application.DTOs;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using Xunit;

namespace UserService.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Configurar JWT settings
        _mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns("YourSuperSecretKeyHereThatIsAtLeast32CharactersLong");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("SocialSyncHub");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("SocialSyncHubUsers");
        
        _authService = new AuthService(_mockRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task LoginAsync_WhenValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var passwordHash = typeof(AuthService)
            .GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.Invoke(null, new object[] { "password123" }) as string ?? string.Empty;

        var user = new User("Test User", "test@example.com", passwordHash)
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("test@example.com");
        result.User.Name.Should().Be("Test User");
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WhenInvalidEmail_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_WhenInvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        var user = new User("Test User", "test@example.com", "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=")
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task RegisterAsync_WhenValidData_ShouldCreateUserAndReturnAuthResponse()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Name = "New User",
            Email = "newuser@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        _mockRepository.Setup(r => r.ExistsByEmailAsync(registerDto.Email))
            .ReturnsAsync(false);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("newuser@example.com");
        result.User.Name.Should().Be("New User");
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Name = "New User",
            Email = "existing@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        _mockRepository.Setup(r => r.ExistsByEmailAsync(registerDto.Email))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task ValidateTokenAsync_WhenValidToken_ShouldReturnTrue()
    {
        // Arrange
        var passwordHash = typeof(AuthService)
            .GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.Invoke(null, new object[] { "password123" }) as string ?? string.Empty;

        var user = new User("Test User", "test@example.com", passwordHash)
        {
            Id = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = "password123"
        };

        var authResponse = await _authService.LoginAsync(loginDto);

        // Act
        var isValid = await _authService.ValidateTokenAsync(authResponse.Token);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateTokenAsync_WhenInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var isValid = await _authService.ValidateTokenAsync(invalidToken);

        // Assert
        isValid.Should().BeFalse();
    }
} 
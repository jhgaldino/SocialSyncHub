using FluentValidation.TestHelper;
using UserService.Application.DTOs;
using UserService.Application.Validators;
using Xunit;
using FluentAssertions;

namespace UserService.Tests.Validators;

public class CreateUserDtoValidatorTests
{
    private readonly CreateUserDtoValidator _validator;

    public CreateUserDtoValidatorTests()
    {
        _validator = new CreateUserDtoValidator();
    }

    [Fact]
    public void Should_Pass_When_Valid_Data()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Name = "João Silva",
            Email = "joao.silva@email.com"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Should_Fail_When_Name_Is_Empty(string name)
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Name = name,
            Email = "joao.silva@email.com"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("J")]
    [InlineData("João Silva com um nome muito longo que excede o limite máximo permitido de caracteres para o campo nome do usuário")]
    public void Should_Fail_When_Name_Length_Invalid(string name)
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Name = name,
            Email = "joao.silva@email.com"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("João123")]
    [InlineData("João@Silva")]
    [InlineData("João_Silva")]
    public void Should_Fail_When_Name_Contains_Invalid_Characters(string name)
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Name = name,
            Email = "joao.silva@email.com"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Should_Fail_When_Email_Is_Empty(string email)
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Name = "João Silva",
            Email = email
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("joao@")]
    [InlineData("@email.com")]
    [InlineData("joao.email.com")]
    public void Should_Fail_When_Email_Format_Invalid(string email)
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Name = "João Silva",
            Email = email
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Fail_When_Email_Too_Long()
    {
        // Arrange
        var emailLocal = new string('a', 141);
        var email = emailLocal + "@email.com"; // total: 151+ caracteres
        var dto = new CreateUserDto
        {
            Name = "João Silva",
            Email = email
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
} 
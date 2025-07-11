using FluentValidation;
using FluentValidation.Results;
using UserService.Application.DTOs;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services;

public interface IValidationService
{
    Task<ValidationResult> ValidateUserEmailAsync(string email, Guid? excludeUserId = null);
    Task<ValidationResult> ValidatePasswordStrengthAsync(string password);
}

public class ValidationService : IValidationService
{
    private readonly IUserRepository _userRepository;

    public ValidationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ValidationResult> ValidateUserEmailAsync(string email, Guid? excludeUserId = null)
    {
        var validator = new InlineValidator<string>();
        
        validator.RuleFor(x => x)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email deve ser válido")
            .MustAsync(async (email, cancellation) =>
            {
                var existingUser = await _userRepository.GetByEmailAsync(email);
                return existingUser == null || (excludeUserId.HasValue && existingUser.Id == excludeUserId.Value);
            }).WithMessage("Este email já está em uso");

        return await validator.ValidateAsync(email);
    }

    public async Task<ValidationResult> ValidatePasswordStrengthAsync(string password)
    {
        var validator = new InlineValidator<string>();
        
        validator.RuleFor(x => x)
            .NotEmpty().WithMessage("A senha é obrigatória")
            .Length(6, 100).WithMessage("A senha deve ter entre 6 e 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial");

        return await validator.ValidateAsync(password);
    }
} 
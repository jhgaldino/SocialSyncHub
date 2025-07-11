using FluentValidation;
using UserService.Application.DTOs;

namespace UserService.Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("O nome deve conter apenas letras e espaços");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email deve ser válido")
            .MaximumLength(150).WithMessage("O email deve ter no máximo 150 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória")
            .Length(6, 100).WithMessage("A senha deve ter entre 6 e 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("A confirmação de senha é obrigatória")
            .Equal(x => x.Password).WithMessage("As senhas não coincidem");
    }
} 
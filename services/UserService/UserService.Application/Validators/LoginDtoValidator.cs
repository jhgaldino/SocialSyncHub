using FluentValidation;
using UserService.Application.DTOs;

namespace UserService.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email deve ser válido")
            .MaximumLength(150).WithMessage("O email deve ter no máximo 150 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória")
            .Length(6, 100).WithMessage("A senha deve ter entre 6 e 100 caracteres");
    }
} 
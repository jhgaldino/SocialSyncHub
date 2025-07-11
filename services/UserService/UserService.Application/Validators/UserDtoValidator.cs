using FluentValidation;
using UserService.Application.DTOs;

namespace UserService.Application.Validators;

public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID é obrigatório");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email deve ser válido")
            .MaximumLength(150).WithMessage("O email deve ter no máximo 150 caracteres");

        RuleFor(x => x.CreatedAt)
            .NotEmpty().WithMessage("A data de criação é obrigatória");
    }
} 
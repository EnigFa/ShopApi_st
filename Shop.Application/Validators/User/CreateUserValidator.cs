using FluentValidation;
using Shop.Application.DTOs.UserDTOs;

namespace Shop.Application.Validators.User;

public class CreateUserValidator : AbstractValidator<UserCreateDTO>
{
    public CreateUserValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email");

        RuleFor(user => user.Password)
            .MinimumLength(6).WithMessage("Password is too short");
    }
}

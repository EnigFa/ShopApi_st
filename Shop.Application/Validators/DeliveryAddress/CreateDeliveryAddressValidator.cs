using FluentValidation;
using Shop.Application.DTOs.DeliveryAddressDTOs;

namespace Shop.Application.Validators.DeliveryAddress;

public class CreateDeliveryAddressValidator : AbstractValidator<DeliveryAddressCreateDTO>
{
    public CreateDeliveryAddressValidator()
    {
        RuleFor(address => address.RecipientName)
            .NotEmpty().WithMessage("Ім'я отримувача обов'язкове")
            .MaximumLength(100);

        RuleFor(address => address.Phone)
            .NotEmpty().WithMessage("Телефон обов'язковий")
            .MaximumLength(30);

        RuleFor(address => address.City)
            .NotEmpty().WithMessage("Місто обов'язкове")
            .MaximumLength(100);

        RuleFor(address => address.Street)
            .NotEmpty().WithMessage("Вулиця обов'язкова")
            .MaximumLength(200);

        RuleFor(address => address.Building)
            .MaximumLength(20);

        RuleFor(address => address.Apartment)
            .MaximumLength(20);

        RuleFor(address => address.PostalCode)
            .MaximumLength(20);
    }
}

using FluentValidation;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Validators.Product;

public class CreateProductValidator : AbstractValidator<ProductCreateDTO>
{
    public CreateProductValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("Назва продукту обов'язкова")
            .MinimumLength(2).WithMessage("Назва продукту має містити щонайменше 2 символи")
            .MaximumLength(200).WithMessage("Назва продукту не може бути довшою за 200 символів");

        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("Ціна має бути більшою за 0");

        RuleFor(product => product.StockQty)
            .GreaterThanOrEqualTo(0).WithMessage("Кількість на складі не може бути від'ємною");

        RuleFor(product => product.CategoryId)
            .GreaterThan(0).WithMessage("Потрібно вказати категорію");

        RuleFor(product => product.Description)
            .MaximumLength(1000).WithMessage("Опис не може бути довшим за 1000 символів");

        RuleFor(product => product.Images)
            .NotNull().WithMessage("Список зображень обов'язковий");

        RuleForEach(product => product.Images)
            .NotEmpty().WithMessage("Адреса зображення не може бути порожньою");
    }
}

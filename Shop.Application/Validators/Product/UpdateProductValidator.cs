using FluentValidation;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Validators.Product;

public class UpdateProductValidator : AbstractValidator<ProductUpdateDTO>
{
    public UpdateProductValidator()
    {
        RuleFor(product => product.Name)
            .MinimumLength(2).WithMessage("Назва продукту має містити щонайменше 2 символи")
            .MaximumLength(200).WithMessage("Назва продукту не може бути довшою за 200 символів")
            .When(product => product.Name != null);

        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("Ціна має бути більшою за 0")
            .When(product => product.Price.HasValue);

        RuleFor(product => product.StockQty)
            .GreaterThanOrEqualTo(0).WithMessage("Кількість на складі не може бути від'ємною")
            .When(product => product.StockQty.HasValue);

        RuleFor(product => product.CategoryId)
            .GreaterThan(0).WithMessage("Потрібно вказати категорію")
            .When(product => product.CategoryId.HasValue);

        RuleFor(product => product.Description)
            .MaximumLength(1000).WithMessage("Опис не може бути довшим за 1000 символів")
            .When(product => product.Description != null);

        RuleForEach(product => product.Images)
            .NotEmpty().WithMessage("Адреса зображення не може бути порожньою")
            .When(product => product.Images != null);
    }
}

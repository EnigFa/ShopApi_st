using FluentValidation;
using Shop.Application.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Validators.Category;

public class CreateCategoryValidator:AbstractValidator<CategoryCreateDTO>
{
    public CreateCategoryValidator()
    {
     
        RuleFor(cat => cat.Name)
            .NotEmpty().WithMessage("Назва категорії обов'язкова")
            .MaximumLength(20).WithMessage("Назва не може бути довшою за 20 символів");


        RuleFor(cat => cat.Slug)
        .NotEmpty().WithMessage("Вкажіть slug категорії")
        .MinimumLength(5).WithMessage("Slug не меньше 5 символів")
        .Matches(@"^[a-zA-Z0-9_-]+$")
        .WithMessage("Slug повинен містити лише латинські літери, цифри та символи - та _");

        RuleFor(cat => cat.ParentId)
            .Must(id => id == null || id > 0)
            .WithMessage("ParentId must be a positive number or null");
    }
}

using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Validators.Product;

namespace ShopTest;

public class ProductValidatorTest
{
    private readonly CreateProductValidator _createValidator = new();
    private readonly UpdateProductValidator _updateValidator = new();

    [Fact]
    public void CreateProductValidator_AcceptsValidProduct()
    {
        var dto = new ProductCreateDTO
        {
            Name = "Ноутбук",
            Price = 19999.50m,
            StockQty = 3,
            CategoryId = 6,
            Description = "Ігровий ноутбук",
            Images = ["products/laptop.webp"]
        };

        var result = _createValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateProductValidator_RejectsInvalidProduct()
    {
        var dto = new ProductCreateDTO
        {
            Name = "А",
            Price = 0,
            StockQty = -1,
            CategoryId = 0,
            Images = [""]
        };

        var result = _createValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
        Assert.Contains(result.Errors, error => error.PropertyName == "Price");
        Assert.Contains(result.Errors, error => error.PropertyName == "StockQty");
        Assert.Contains(result.Errors, error => error.PropertyName == "CategoryId");
        Assert.Contains(result.Errors, error => error.PropertyName == "Images[0]");
    }

    [Fact]
    public void UpdateProductValidator_AcceptsPartialUpdate()
    {
        var dto = new ProductUpdateDTO
        {
            Price = 1500
        };

        var result = _updateValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateProductValidator_RejectsInvalidFields()
    {
        var dto = new ProductUpdateDTO
        {
            Name = " ",
            Price = -10,
            StockQty = -2,
            CategoryId = 0
        };

        var result = _updateValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Price");
        Assert.Contains(result.Errors, error => error.PropertyName == "StockQty");
        Assert.Contains(result.Errors, error => error.PropertyName == "CategoryId");
    }
}

using System.ComponentModel.DataAnnotations;


namespace Shop.Application.DTOs.ProductDTOs;

public class ProductCreateDTO
{
    /// <summary>
    /// Назва продукту
    /// </summary>
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ціна продукту
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Опис продукту
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Кількість товару на складі
    /// </summary>
    public int StockQty { get; set; }

    /// <summary>
    /// Чи активний продукт
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Ідентифікатор категорії
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Список URL фотографій продукту
    /// </summary>
    public ICollection<string> Images { get; set; } = [];
}

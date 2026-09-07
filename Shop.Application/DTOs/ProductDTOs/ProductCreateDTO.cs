using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.ProductDTOs;

public class ProductCreateDTO
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public int StockQty { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public int CategoryId { get; set; }
}

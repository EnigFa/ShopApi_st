namespace Shop.Application.DTOs.ProductDTOs;

public class ProductReadDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQty { get; set; }

    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }
}

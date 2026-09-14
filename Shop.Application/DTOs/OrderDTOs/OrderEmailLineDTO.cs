namespace Shop.Application.DTOs.OrderDTOs;

public class OrderEmailLineDTO
{
    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Count { get; set; }

    public decimal LineTotal => Price * Count;
}

using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.OrderDTOs;

public class OrderItemCreateDTO
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Count { get; set; }
}

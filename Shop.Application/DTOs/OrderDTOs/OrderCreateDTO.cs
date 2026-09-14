using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.OrderDTOs;

public class OrderCreateDTO
{
    [Required]
    public Guid UserId { get; set; }

    public bool Paid { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemCreateDTO> Items { get; set; } = [];
}

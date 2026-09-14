using Shop.Domain.Enums;

namespace Shop.Application.DTOs.ProductDTOs;

public class ProductFeedbackReadDTO
{
    public string Id { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public ProductFeedbackKind Kind { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorEmail { get; set; }

    public int? Rating { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

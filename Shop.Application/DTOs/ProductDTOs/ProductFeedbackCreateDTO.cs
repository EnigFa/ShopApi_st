using Shop.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shop.Application.DTOs.ProductDTOs;

public class ProductFeedbackCreateDTO
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductFeedbackKind Kind { get; set; }

    [MaxLength(100)]
    public string? AuthorName { get; set; }

    [EmailAddress]
    public string? AuthorEmail { get; set; }

    [Range(1, 5)]
    public int? Rating { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(2000)]
    public string Text { get; set; } = string.Empty;
}

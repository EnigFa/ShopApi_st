using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("delivery_addresses")]
public class DeliveryAddress : BaseEntity
{
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [Column("recipient_name")]
    public string RecipientName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    [Column("phone")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("city")]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("street")]
    public string Street { get; set; } = string.Empty;

    [MaxLength(20)]
    [Column("building")]
    public string? Building { get; set; }

    [MaxLength(20)]
    [Column("apartment")]
    public string? Apartment { get; set; }

    [MaxLength(20)]
    [Column("postal_code")]
    public string? PostalCode { get; set; }

    [Column("is_default")]
    public bool IsDefault { get; set; }
}

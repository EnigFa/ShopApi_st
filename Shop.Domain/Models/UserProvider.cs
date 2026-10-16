using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("usersProviders")]
public class UserProvider
{
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    [Column("provider_id")]
    public int ProviderId { get; set; }

    public Provider Provider { get; set; } = null!;

    [Required]
    [Column("number_provider")]
    public string NumberProvider { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shop.Domain.Models;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("token")]
    public string Token { get; set; } = string.Empty;

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("is_revoked")]
    public bool IsRevoked { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}

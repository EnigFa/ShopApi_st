using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.UserDTOs;

public class ResetPasswordDTO
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [MinLength(5)]
    public string NewPassword { get; set; } = null!;
}

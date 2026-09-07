using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.UserDTOs;

public class ForgotPasswordDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.DTOs.AuthDTOs;

public class AuthResponseDto
{
    public string RefreshToken { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
}

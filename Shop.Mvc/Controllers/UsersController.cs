using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces.Services;

namespace Shop.Mvc.Controllers;

public class UsersController(IAuthService authService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var users = await authService.GetAllUsersAsync(cancellationToken);
        return View(users);
    }
}

using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces.Services;

namespace Shop.Mvc.Controllers;

public class CategoriesController(ICategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetAllCategoriesAsync(cancellationToken) ?? [];
        return View(categories);
    }
}

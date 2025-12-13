using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Menu;

[Authorize(Roles = "Admin")]
public class AddModel : PageModel
{
    private readonly IMenuService _menuService;

    public AddModel(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [BindProperty]
    public AddMenuItemViewModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var menu = new WeeklyMenu
        {
            DayOfWeek = Input.DayOfWeek,
            MealType = Input.MealType,
            DishName = Input.DishName,
            Price = Input.Price
        };

        await _menuService.AddDishAsync(menu);

        TempData["ToastSuccess"] = $"Menu item '{Input.DishName}' added successfully!";
        return RedirectToPage("Index");
    }
}
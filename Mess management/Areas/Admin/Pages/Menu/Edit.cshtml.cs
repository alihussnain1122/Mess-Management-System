using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Menu;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly IMenuService _menuService;

    public EditModel(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [BindProperty]
    public EditMenuItemViewModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var dish = await _menuService.GetDishByIdAsync(id);
        if (dish == null)
            return NotFound();

        Input = new EditMenuItemViewModel
        {
            Id = dish.Id,
            DayOfWeek = dish.DayOfWeek,
            MealType = dish.MealType,
            DishName = dish.DishName,
            Price = dish.Price
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var dish = new WeeklyMenu
        {
            Id = Input.Id,
            DayOfWeek = Input.DayOfWeek,
            MealType = Input.MealType,
            DishName = Input.DishName,
            Price = Input.Price
        };

        await _menuService.UpdateDishAsync(dish);

        TempData["ToastSuccess"] = $"Menu item '{Input.DishName}' updated successfully!";
        return RedirectToPage("Index");
    }
}
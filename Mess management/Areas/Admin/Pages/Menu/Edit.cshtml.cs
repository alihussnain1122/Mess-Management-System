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
            MenuDate = dish.MenuDate,
            MealType = dish.MealType,
            DishName = dish.DishName,
            Price = dish.Price,
            IsSpecificDate = dish.MenuDate.HasValue
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Validation based on menu type
        if (Input.IsSpecificDate)
        {
            if (!Input.MenuDate.HasValue)
            {
                TempData["ToastError"] = "Please select a date for the menu.";
                return Page();
            }

            // Check if a dish already exists for this specific date and meal type
            var existingDish = await _menuService.DishExistsForDateMealAsync(Input.MenuDate.Value, Input.MealType, Input.Id);
            if (existingDish)
            {
                var mealName = Input.MealType.ToString();
                var dateName = Input.MenuDate.Value.ToString("MMMM dd, yyyy");
                TempData["ToastError"] = $"A {mealName} dish already exists for {dateName}. Please choose a different date or meal type.";
                return Page();
            }
        }
        else
        {
            // Check if a dish already exists for this day and meal type (excluding current dish)
            var existingDish = await _menuService.DishExistsForMealAsync(Input.DayOfWeek, Input.MealType, Input.Id);
            if (existingDish)
            {
                var mealName = Input.MealType.ToString();
                var dayName = Input.DayOfWeek.ToString();
                TempData["ToastError"] = $"A {mealName} dish already exists for {dayName}. Please choose a different day or meal type.";
                return Page();
            }
        }

        var dish = new WeeklyMenu
        {
            Id = Input.Id,
            DayOfWeek = Input.IsSpecificDate ? Input.MenuDate!.Value.DayOfWeek : Input.DayOfWeek,
            MenuDate = Input.IsSpecificDate ? Input.MenuDate : null,
            MealType = Input.MealType,
            DishName = Input.DishName,
            Price = Input.Price
        };

        await _menuService.UpdateDishAsync(dish);

        TempData["ToastSuccess"] = $"Menu item '{Input.DishName}' updated successfully!";
        return RedirectToPage("Index");
    }
}
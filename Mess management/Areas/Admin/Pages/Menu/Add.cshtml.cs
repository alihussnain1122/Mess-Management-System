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

        // If specific date is selected, validate the date
        if (Input.IsSpecificDate)
        {
            if (!Input.MenuDate.HasValue)
            {
                TempData["ToastError"] = "Please select a date for the menu.";
                return Page();
            }

            // Check date is within allowed range (today to 1 month ahead)
            var maxDate = DateTime.Now.Date.AddDays(31);
            if (Input.MenuDate.Value.Date < DateTime.Now.Date || Input.MenuDate.Value.Date > maxDate)
            {
                TempData["ToastError"] = "Menu date must be between today and 1 month ahead.";
                return Page();
            }

            // Check if a dish already exists for this specific date and meal type
            var existingDish = await _menuService.DishExistsForDateMealAsync(Input.MenuDate.Value, Input.MealType);
            if (existingDish)
            {
                var mealName = Input.MealType.ToString();
                var dateName = Input.MenuDate.Value.ToString("MMMM dd, yyyy");
                TempData["ToastError"] = $"A {mealName} dish already exists for {dateName}. Please edit or delete the existing dish first.";
                return Page();
            }
        }
        else
        {
            // Check if a dish already exists for this day and meal type (template)
            var existingDish = await _menuService.DishExistsForMealAsync(Input.DayOfWeek, Input.MealType);
            if (existingDish)
            {
                var mealName = Input.MealType.ToString();
                var dayName = Input.DayOfWeek.ToString();
                TempData["ToastError"] = $"A {mealName} dish already exists for {dayName}. Please edit or delete the existing dish first.";
                return Page();
            }
        }

        var menu = new WeeklyMenu
        {
            DayOfWeek = Input.IsSpecificDate ? Input.MenuDate!.Value.DayOfWeek : Input.DayOfWeek,
            MenuDate = Input.IsSpecificDate ? Input.MenuDate : null,
            MealType = Input.MealType,
            DishName = Input.DishName,
            Price = Input.Price
        };

        await _menuService.AddDishAsync(menu);

        var successMsg = Input.IsSpecificDate 
            ? $"Menu item '{Input.DishName}' added for {Input.MenuDate!.Value:MMMM dd, yyyy}!"
            : $"Menu item '{Input.DishName}' added successfully!";
        TempData["ToastSuccess"] = successMsg;
        return RedirectToPage("Index");
    }
}
using System.ComponentModel.DataAnnotations;
using MessManagement.Models;

namespace MessManagement.ViewModels;

public class AddMenuItemViewModel
{
    [Required(ErrorMessage = "Day of week is required")]
    public DayOfWeek DayOfWeek { get; set; }

    [Required(ErrorMessage = "Dish name is required")]
    [StringLength(100)]
    public string DishName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Meal type is required")]
    public MealType MealType { get; set; }
}

public class EditMenuItemViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Day of week is required")]
    public DayOfWeek DayOfWeek { get; set; }

    [Required(ErrorMessage = "Dish name is required")]
    [StringLength(100)]
    public string DishName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Meal type is required")]
    public MealType MealType { get; set; }
}

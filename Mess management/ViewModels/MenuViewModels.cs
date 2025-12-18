using System.ComponentModel.DataAnnotations;
using MessManagement.Models;

namespace MessManagement.ViewModels;

public class AddMenuItemViewModel
{
    [Required(ErrorMessage = "Day of week is required")]
    public DayOfWeek DayOfWeek { get; set; }

    // Optional: Specific date for monthly planning
    [DataType(DataType.Date)]
    public DateTime? MenuDate { get; set; }

    [Required(ErrorMessage = "Dish name is required")]
    [StringLength(100)]
    public string DishName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Meal type is required")]
    public MealType MealType { get; set; }

    // Menu type: Template (recurring weekly) or Specific Date
    public bool IsSpecificDate { get; set; } = false;
}

public class EditMenuItemViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Day of week is required")]
    public DayOfWeek DayOfWeek { get; set; }

    [DataType(DataType.Date)]
    public DateTime? MenuDate { get; set; }

    [Required(ErrorMessage = "Dish name is required")]
    [StringLength(100)]
    public string DishName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Meal type is required")]
    public MealType MealType { get; set; }

    public bool IsSpecificDate { get; set; } = false;
}

// View model for detailed bill with dishes
public class DetailedBillViewModel
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public int Month { get; set; }
    public int Year { get; set; }
    public string MonthName { get; set; } = "";
    public List<DailyMealDetail> DailyMeals { get; set; } = new();
    public decimal TotalMealCost { get; set; }
    public decimal TotalTeaCost { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
}

public class DailyMealDetail
{
    public DateTime Date { get; set; }
    public string DayName { get; set; } = "";
    public List<MealDetail> Meals { get; set; } = new();
    public decimal DayTotal { get; set; }
}

public class MealDetail
{
    public MealType MealType { get; set; }
    public string DishName { get; set; } = "";
    public decimal Price { get; set; }
    public bool WasPresent { get; set; }
}


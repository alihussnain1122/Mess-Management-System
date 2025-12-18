using MessManagement.Models;

namespace MessManagement.Interfaces;

public interface IMenuService
{
    Task<IEnumerable<WeeklyMenu>> GetWeeklyMenuAsync();
    Task<IEnumerable<WeeklyMenu>> GetMenuByDayAsync(DayOfWeek dayOfWeek);
    Task<IEnumerable<WeeklyMenu>> GetMenuByDateAsync(DateTime date);
    Task<IEnumerable<WeeklyMenu>> GetMenuForDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<WeeklyMenu?> GetDishByIdAsync(int id);
    Task<WeeklyMenu> AddDishAsync(WeeklyMenu menu);
    Task<WeeklyMenu> UpdateDishAsync(WeeklyMenu menu);
    Task<bool> DeleteDishAsync(int id);
    Task<decimal> GetDailyMenuCostAsync(DayOfWeek dayOfWeek);
    Task<decimal> GetMenuCostForDateAsync(DateTime date);
    Task<Dictionary<DayOfWeek, decimal>> GetWeeklyMenuCostSummaryAsync();
    Task<bool> DishExistsForMealAsync(DayOfWeek dayOfWeek, MealType mealType, int? excludeId = null);
    Task<bool> DishExistsForDateMealAsync(DateTime date, MealType mealType, int? excludeId = null);
}

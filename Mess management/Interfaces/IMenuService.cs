using MessManagement.Models;

namespace MessManagement.Interfaces;

public interface IMenuService
{
    Task<IEnumerable<WeeklyMenu>> GetWeeklyMenuAsync();
    Task<IEnumerable<WeeklyMenu>> GetMenuByDayAsync(DayOfWeek dayOfWeek);
    Task<WeeklyMenu?> GetDishByIdAsync(int id);
    Task<WeeklyMenu> AddDishAsync(WeeklyMenu menu);
    Task<WeeklyMenu> UpdateDishAsync(WeeklyMenu menu);
    Task<bool> DeleteDishAsync(int id);
    Task<decimal> GetDailyMenuCostAsync(DayOfWeek dayOfWeek);
    Task<Dictionary<DayOfWeek, decimal>> GetWeeklyMenuCostSummaryAsync();
}

using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Services;

public class MenuService : IMenuService
{
    private readonly MessDbContext _context;

    public MenuService(MessDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WeeklyMenu>> GetWeeklyMenuAsync()
    {
        // Returns template menus (those without specific dates)
        return await _context.WeeklyMenus
            .Where(m => m.MenuDate == null)
            .OrderBy(m => m.DayOfWeek)
            .ThenBy(m => m.MealType)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeeklyMenu>> GetMenuByDayAsync(DayOfWeek dayOfWeek)
    {
        return await _context.WeeklyMenus
            .Where(m => m.MenuDate == null && m.DayOfWeek == dayOfWeek)
            .OrderBy(m => m.MealType)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeeklyMenu>> GetMenuByDateAsync(DateTime date)
    {
        // First try to get menu for specific date
        var specificMenu = await _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate.Value.Date == date.Date)
            .OrderBy(m => m.MealType)
            .ToListAsync();

        if (specificMenu.Any())
            return specificMenu;

        // Fall back to template menu for that day of week
        return await _context.WeeklyMenus
            .Where(m => m.MenuDate == null && m.DayOfWeek == date.DayOfWeek)
            .OrderBy(m => m.MealType)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeeklyMenu>> GetMenuForDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        // Get all specific date menus in range
        var specificMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate >= startDate && m.MenuDate <= endDate)
            .ToListAsync();

        // Get template menus
        var templateMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate == null)
            .ToListAsync();

        return specificMenus.Concat(templateMenus)
            .OrderBy(m => m.MenuDate ?? DateTime.MaxValue)
            .ThenBy(m => m.DayOfWeek)
            .ThenBy(m => m.MealType);
    }

    public async Task<WeeklyMenu?> GetDishByIdAsync(int id)
    {
        return await _context.WeeklyMenus.FindAsync(id);
    }

    public async Task<WeeklyMenu> AddDishAsync(WeeklyMenu menu)
    {
        menu.CreatedAt = DateTime.UtcNow;
        
        // If MenuDate is set, derive DayOfWeek from it
        if (menu.MenuDate.HasValue)
        {
            menu.DayOfWeek = menu.MenuDate.Value.DayOfWeek;
        }
        
        _context.WeeklyMenus.Add(menu);
        await _context.SaveChangesAsync();
        
        return menu;
    }

    public async Task<WeeklyMenu> UpdateDishAsync(WeeklyMenu menu)
    {
        var existingDish = await _context.WeeklyMenus.FindAsync(menu.Id);
        
        if (existingDish == null)
            throw new ArgumentException("Dish not found", nameof(menu));

        existingDish.DayOfWeek = menu.MenuDate?.DayOfWeek ?? menu.DayOfWeek;
        existingDish.MenuDate = menu.MenuDate;
        existingDish.DishName = menu.DishName;
        existingDish.Price = menu.Price;
        existingDish.MealType = menu.MealType;
        existingDish.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        return existingDish;
    }

    public async Task<bool> DeleteDishAsync(int id)
    {
        var dish = await _context.WeeklyMenus.FindAsync(id);
        
        if (dish == null)
            return false;

        _context.WeeklyMenus.Remove(dish);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<decimal> GetDailyMenuCostAsync(DayOfWeek dayOfWeek)
    {
        return await _context.WeeklyMenus
            .Where(m => m.MenuDate == null && m.DayOfWeek == dayOfWeek)
            .SumAsync(m => m.Price);
    }

    public async Task<decimal> GetMenuCostForDateAsync(DateTime date)
    {
        // First check for specific date menu
        var specificMenuCost = await _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate.Value.Date == date.Date)
            .SumAsync(m => m.Price);

        if (specificMenuCost > 0)
            return specificMenuCost;

        // Fall back to template menu
        return await _context.WeeklyMenus
            .Where(m => m.MenuDate == null && m.DayOfWeek == date.DayOfWeek)
            .SumAsync(m => m.Price);
    }

    public async Task<Dictionary<DayOfWeek, decimal>> GetWeeklyMenuCostSummaryAsync()
    {
        var menuItems = await _context.WeeklyMenus
            .Where(m => m.MenuDate == null)
            .ToListAsync();
        
        return menuItems
            .GroupBy(m => m.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Sum(m => m.Price));
    }

    public async Task<bool> DishExistsForMealAsync(DayOfWeek dayOfWeek, MealType mealType, int? excludeId = null)
    {
        var query = _context.WeeklyMenus
            .Where(m => m.MenuDate == null && m.DayOfWeek == dayOfWeek && m.MealType == mealType);
        
        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> DishExistsForDateMealAsync(DateTime date, MealType mealType, int? excludeId = null)
    {
        var query = _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate.Value.Date == date.Date && m.MealType == mealType);
        
        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
}

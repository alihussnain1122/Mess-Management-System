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
        return await _context.WeeklyMenus
            .OrderBy(m => m.DayOfWeek)
            .ThenBy(m => m.MealType)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeeklyMenu>> GetMenuByDayAsync(DayOfWeek dayOfWeek)
    {
        return await _context.WeeklyMenus
            .Where(m => m.DayOfWeek == dayOfWeek)
            .OrderBy(m => m.MealType)
            .ToListAsync();
    }

    public async Task<WeeklyMenu?> GetDishByIdAsync(int id)
    {
        return await _context.WeeklyMenus.FindAsync(id);
    }

    public async Task<WeeklyMenu> AddDishAsync(WeeklyMenu menu)
    {
        menu.CreatedAt = DateTime.UtcNow;
        
        _context.WeeklyMenus.Add(menu);
        await _context.SaveChangesAsync();
        
        return menu;
    }

    public async Task<WeeklyMenu> UpdateDishAsync(WeeklyMenu menu)
    {
        var existingDish = await _context.WeeklyMenus.FindAsync(menu.Id);
        
        if (existingDish == null)
            throw new ArgumentException("Dish not found", nameof(menu));

        existingDish.DayOfWeek = menu.DayOfWeek;
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
            .Where(m => m.DayOfWeek == dayOfWeek)
            .SumAsync(m => m.Price);
    }

    public async Task<Dictionary<DayOfWeek, decimal>> GetWeeklyMenuCostSummaryAsync()
    {
        var menuItems = await _context.WeeklyMenus.ToListAsync();
        
        return menuItems
            .GroupBy(m => m.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Sum(m => m.Price));
    }
}

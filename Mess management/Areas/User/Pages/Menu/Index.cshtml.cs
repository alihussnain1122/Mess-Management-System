using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Areas.User.Pages.Menu;

[Authorize(Roles = "User")]
public class IndexModel : PageModel
{
    private readonly IMenuService _menuService;
    private readonly MessDbContext _context;

    public IndexModel(IMenuService menuService, MessDbContext context)
    {
        _menuService = menuService;
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string ViewMode { get; set; } = "weekly"; // "weekly" or "monthly"

    [BindProperty(SupportsGet = true)]
    public int? Month { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Year { get; set; }

    public IEnumerable<WeeklyMenu>? WeeklyMenuItems { get; set; }
    public List<DailyMenuView> MonthlyMenuItems { get; set; } = new();
    public string MonthName { get; set; } = "";
    public int SelectedMonth { get; set; }
    public int SelectedYear { get; set; }

    public async Task OnGetAsync()
    {
        SelectedMonth = Month ?? DateTime.Now.Month;
        SelectedYear = Year ?? DateTime.Now.Year;
        MonthName = new DateTime(SelectedYear, SelectedMonth, 1).ToString("MMMM yyyy");

        if (ViewMode == "weekly")
        {
            // Get template menus for weekly view
            WeeklyMenuItems = await _menuService.GetWeeklyMenuAsync();
        }
        else
        {
            // Get monthly view with specific dates and fallback to templates
            var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get specific date menus for the month
            var specificMenus = await _context.WeeklyMenus
                .Where(m => m.MenuDate != null && m.MenuDate >= startDate && m.MenuDate <= endDate)
                .ToListAsync();

            // Get template menus
            var templateMenus = await _context.WeeklyMenus
                .Where(m => m.MenuDate == null)
                .ToListAsync();

            // Build daily menu for each day of the month
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dayMenu = new DailyMenuView
                {
                    Date = date,
                    DayName = date.DayOfWeek.ToString(),
                    IsToday = date.Date == DateTime.Now.Date
                };

                // Check for specific date menu first
                var specificForDate = specificMenus.Where(m => m.MenuDate!.Value.Date == date.Date).ToList();
                if (specificForDate.Any())
                {
                    dayMenu.Meals = specificForDate.Select(m => new MealView
                    {
                        MealType = m.MealType,
                        DishName = m.DishName,
                        Price = m.Price,
                        IsSpecificDate = true
                    }).OrderBy(m => m.MealType).ToList();
                }
                else
                {
                    // Use template menu
                    var templateForDay = templateMenus.Where(m => m.DayOfWeek == date.DayOfWeek).ToList();
                    dayMenu.Meals = templateForDay.Select(m => new MealView
                    {
                        MealType = m.MealType,
                        DishName = m.DishName,
                        Price = m.Price,
                        IsSpecificDate = false
                    }).OrderBy(m => m.MealType).ToList();
                }

                dayMenu.DayTotal = dayMenu.Meals.Sum(m => m.Price);
                MonthlyMenuItems.Add(dayMenu);
            }
        }
    }
}

public class DailyMenuView
{
    public DateTime Date { get; set; }
    public string DayName { get; set; } = "";
    public bool IsToday { get; set; }
    public List<MealView> Meals { get; set; } = new();
    public decimal DayTotal { get; set; }
}

public class MealView
{
    public MealType MealType { get; set; }
    public string DishName { get; set; } = "";
    public decimal Price { get; set; }
    public bool IsSpecificDate { get; set; }
}

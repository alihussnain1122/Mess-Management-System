using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.User.Pages.Menu;

[Authorize(Roles = "User")]
public class WeeklyModel : PageModel
{
    private readonly IMenuService _menuService;

    public WeeklyModel(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public Dictionary<DateTime, List<WeeklyMenu>> WeeklyMenuData { get; set; } = new();

    public async Task OnGetAsync()
    {
        var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(6);

        // Run queries sequentially to avoid DbContext concurrency issues
        for (var date = startOfWeek; date <= endOfWeek; date = date.AddDays(1))
        {
            var items = await _menuService.GetMenuByDayAsync(date.DayOfWeek);
            WeeklyMenuData[date] = items.ToList();
        }
    }
}
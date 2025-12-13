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

        var tasks = new List<Task<(DateTime date, IEnumerable<WeeklyMenu> items)>>();
        
        for (var date = startOfWeek; date <= endOfWeek; date = date.AddDays(1))
        {
            var currentDate = date;
            tasks.Add(GetMenuForDate(currentDate));
        }

        var results = await Task.WhenAll(tasks);
        
        foreach (var result in results.OrderBy(r => r.date))
        {
            WeeklyMenuData[result.date] = result.items.ToList();
        }
    }

    private async Task<(DateTime date, IEnumerable<WeeklyMenu> items)> GetMenuForDate(DateTime date)
    {
        var items = await _menuService.GetMenuByDayAsync(date.DayOfWeek);
        return (date, items);
    }
}
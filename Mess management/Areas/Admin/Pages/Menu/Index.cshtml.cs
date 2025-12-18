using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Areas.Admin.Pages.Menu;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IMenuService _menuService;
    private readonly MessDbContext _context;

    public IndexModel(IMenuService menuService, MessDbContext context)
    {
        _menuService = menuService;
        _context = context;
    }

    public IEnumerable<WeeklyMenu>? TemplateMenuItems { get; set; }
    public IEnumerable<WeeklyMenu>? SpecificDateMenuItems { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string ViewMode { get; set; } = "template"; // "template" or "specific"

    public async Task OnGetAsync()
    {
        // Get template menus (weekly recurring)
        TemplateMenuItems = await _menuService.GetWeeklyMenuAsync();
        
        // Get specific date menus (next 30 days)
        var startDate = DateTime.Now.Date;
        var endDate = startDate.AddDays(30);
        SpecificDateMenuItems = await _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate >= startDate && m.MenuDate <= endDate)
            .OrderBy(m => m.MenuDate)
            .ThenBy(m => m.MealType)
            .ToListAsync();
    }
}
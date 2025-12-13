using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Menu;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IMenuService _menuService;

    public IndexModel(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public IEnumerable<WeeklyMenu>? MenuItems { get; set; }

    public async Task OnGetAsync()
    {
        MenuItems = await _menuService.GetWeeklyMenuAsync();
    }
}
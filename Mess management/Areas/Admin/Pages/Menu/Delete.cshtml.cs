using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Menu;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly IMenuService _menuService;

    public DeleteModel(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public string DishName { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var dish = await _menuService.GetDishByIdAsync(id);
        if (dish == null)
            return NotFound();

        DishName = dish.DishName;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _menuService.DeleteDishAsync(id);
        return RedirectToPage("Index");
    }
}
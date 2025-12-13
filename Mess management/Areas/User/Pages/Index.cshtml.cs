using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.User.Pages;

[Authorize(Roles = "User")]
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("Dashboard");
    }
}
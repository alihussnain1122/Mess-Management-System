using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Pages;

[AllowAnonymous] // Explicitly allow unauthenticated access to landing page
public class IndexModel : PageModel
{
    public void OnGet()
    {
        // Show landing page
    }
}

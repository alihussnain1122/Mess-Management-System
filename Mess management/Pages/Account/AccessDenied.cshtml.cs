using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Pages.Account;

[AllowAnonymous] // Explicitly allow access to show the access denied message
public class AccessDeniedModel : PageModel
{
    public void OnGet()
    {
    }
}

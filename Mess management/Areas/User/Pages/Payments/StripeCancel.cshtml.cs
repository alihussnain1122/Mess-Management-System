using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.User.Pages.Payments;

[Authorize(Roles = "User")]
public class StripeCancelModel : PageModel
{
    public void OnGet()
    {
    }
}

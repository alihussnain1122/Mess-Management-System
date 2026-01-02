using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;
using System.Security.Claims;

namespace MessManagement.Areas.User.Pages.Suggestions;

[Authorize(Roles = "User")]
public class DetailsModel : PageModel
{
    private readonly MessDbContext _context;

    public DetailsModel(MessDbContext context)
    {
        _context = context;
    }

    public Suggestion? Suggestion { get; set; }
    public string? AdminName { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return RedirectToPage("/Account/Login");

        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return RedirectToPage("/Account/Login");

        Suggestion = await _context.Suggestions
            .Include(s => s.RespondedByUser)
            .FirstOrDefaultAsync(s => s.SuggestionId == id && s.MemberId == member.MemberId);

        if (Suggestion == null)
            return NotFound();

        if (Suggestion.RespondedByUser != null)
        {
            AdminName = Suggestion.RespondedByUser.Username;
        }

        return Page();
    }
}

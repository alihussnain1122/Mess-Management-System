using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;
using System.Security.Claims;

namespace MessManagement.Areas.User.Pages.Suggestions;

[Authorize(Roles = "User")]
public class IndexModel : PageModel
{
    private readonly MessDbContext _context;

    public IndexModel(MessDbContext context)
    {
        _context = context;
    }

    public List<Suggestion> MySuggestions { get; set; } = new();
    public int PendingCount { get; set; }
    public int ResolvedCount { get; set; }
    public int TotalCount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return RedirectToPage("/Account/Login");

        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return RedirectToPage("/Account/Login");

        MySuggestions = await _context.Suggestions
            .Where(s => s.MemberId == member.MemberId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        TotalCount = MySuggestions.Count;
        PendingCount = MySuggestions.Count(s => s.Status == SuggestionStatus.Pending || s.Status == SuggestionStatus.UnderReview);
        ResolvedCount = MySuggestions.Count(s => s.Status == SuggestionStatus.Resolved || s.Status == SuggestionStatus.Implemented);

        return Page();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;

namespace MessManagement.Areas.Admin.Pages.Suggestions;

[Authorize(Roles = "Admin")]
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
        Suggestion = await _context.Suggestions
            .Include(s => s.Member)
            .Include(s => s.RespondedByUser)
            .FirstOrDefaultAsync(s => s.SuggestionId == id);

        if (Suggestion == null)
            return NotFound();

        if (Suggestion.RespondedByUser != null)
        {
            AdminName = Suggestion.RespondedByUser.Username;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var suggestion = await _context.Suggestions.FindAsync(id);
        
        if (suggestion != null)
        {
            _context.Suggestions.Remove(suggestion);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Suggestion deleted successfully.";
        }

        return RedirectToPage("Index");
    }
}

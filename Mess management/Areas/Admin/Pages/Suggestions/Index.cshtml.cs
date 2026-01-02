using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;

namespace MessManagement.Areas.Admin.Pages.Suggestions;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly MessDbContext _context;

    public IndexModel(MessDbContext context)
    {
        _context = context;
    }

    public List<Suggestion> Suggestions { get; set; } = new();
    
    // Filters
    public string? StatusFilter { get; set; }
    public string? CategoryFilter { get; set; }
    
    // Stats
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int UnderReviewCount { get; set; }
    public int ResolvedCount { get; set; }

    public async Task<IActionResult> OnGetAsync(string? status, string? category)
    {
        StatusFilter = status;
        CategoryFilter = category;

        var query = _context.Suggestions
            .Include(s => s.Member)
            .AsQueryable();

        // Apply status filter
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SuggestionStatus>(status, out var statusEnum))
        {
            query = query.Where(s => s.Status == statusEnum);
        }

        // Apply category filter
        if (!string.IsNullOrEmpty(category) && Enum.TryParse<SuggestionCategory>(category, out var categoryEnum))
        {
            query = query.Where(s => s.Category == categoryEnum);
        }

        Suggestions = await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        // Get stats (unfiltered)
        var allSuggestions = await _context.Suggestions.ToListAsync();
        TotalCount = allSuggestions.Count;
        PendingCount = allSuggestions.Count(s => s.Status == SuggestionStatus.Pending);
        UnderReviewCount = allSuggestions.Count(s => s.Status == SuggestionStatus.UnderReview);
        ResolvedCount = allSuggestions.Count(s => s.Status == SuggestionStatus.Resolved || s.Status == SuggestionStatus.Implemented);

        return Page();
    }
}

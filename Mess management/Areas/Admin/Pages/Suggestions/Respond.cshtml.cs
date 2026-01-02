using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace MessManagement.Areas.Admin.Pages.Suggestions;

[Authorize(Roles = "Admin")]
public class RespondModel : PageModel
{
    private readonly MessDbContext _context;

    public RespondModel(MessDbContext context)
    {
        _context = context;
    }

    public Suggestion? Suggestion { get; set; }

    [BindProperty]
    public ResponseInput Input { get; set; } = new();

    public class ResponseInput
    {
        [Required(ErrorMessage = "Please select a status")]
        public SuggestionStatus Status { get; set; }

        [Required(ErrorMessage = "Response message is required")]
        [StringLength(1000, ErrorMessage = "Response cannot exceed 1000 characters")]
        public string AdminResponse { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Suggestion = await _context.Suggestions
            .Include(s => s.Member)
            .FirstOrDefaultAsync(s => s.SuggestionId == id);

        if (Suggestion == null)
            return NotFound();

        // Pre-fill existing response if any
        Input.Status = Suggestion.Status == SuggestionStatus.Pending ? SuggestionStatus.Resolved : Suggestion.Status;
        Input.AdminResponse = Suggestion.AdminResponse ?? "";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Suggestion = await _context.Suggestions
            .Include(s => s.Member)
            .FirstOrDefaultAsync(s => s.SuggestionId == id);

        if (Suggestion == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdClaim, out int userId);

        Suggestion.Status = Input.Status;
        Suggestion.AdminResponse = Input.AdminResponse;
        Suggestion.RespondedByUserId = userId;
        Suggestion.RespondedAt = DateTime.UtcNow;
        Suggestion.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Response sent successfully!";
        return RedirectToPage("Index");
    }
}

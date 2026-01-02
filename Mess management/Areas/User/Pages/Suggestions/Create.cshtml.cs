using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace MessManagement.Areas.User.Pages.Suggestions;

[Authorize(Roles = "User")]
public class CreateModel : PageModel
{
    private readonly MessDbContext _context;

    public CreateModel(MessDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public SuggestionInput Input { get; set; } = new();

    public class SuggestionInput
    {
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(100, ErrorMessage = "Subject cannot exceed 100 characters")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category")]
        public SuggestionCategory Category { get; set; } = SuggestionCategory.General;

        public bool IsAnonymous { get; set; } = false;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return RedirectToPage("/Account/Login");

        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return RedirectToPage("/Account/Login");

        var suggestion = new Suggestion
        {
            MemberId = member.MemberId,
            Subject = Input.Subject,
            Message = Input.Message,
            Category = Input.Category,
            IsAnonymous = Input.IsAnonymous,
            Status = SuggestionStatus.Pending,
            Priority = SuggestionPriority.Normal,
            CreatedAt = DateTime.UtcNow
        };

        _context.Suggestions.Add(suggestion);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Your suggestion has been submitted successfully!";
        return RedirectToPage("Index");
    }
}

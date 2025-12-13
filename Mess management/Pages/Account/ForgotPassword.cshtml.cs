using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MessManagement.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly MessDbContext _context;
    private readonly IEmailService _emailService;

    public ForgotPasswordModel(MessDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Find user by email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);

        if (user == null)
        {
            ErrorMessage = "No account found with this email address.";
            return Page();
        }

        // Generate 6-digit code
        var random = new Random();
        var code = random.Next(100000, 999999).ToString();

        // Invalidate any existing unused tokens for this user
        var existingTokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.IsUsed)
            .ToListAsync();
        
        foreach (var token in existingTokens)
        {
            token.IsUsed = true;
        }

        // Create new token
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Code = code,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false
        };

        _context.PasswordResetTokens.Add(resetToken);
        await _context.SaveChangesAsync();

        // Send email
        var userName = user.Member?.FullName ?? user.Username;
        await _emailService.SendPasswordResetCodeAsync(Input.Email, userName, code);

        // Redirect to verify page
        TempData["ResetEmail"] = Input.Email;
        return RedirectToPage("VerifyResetCode");
    }
}

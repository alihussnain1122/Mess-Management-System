using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Helpers;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace MessManagement.Pages.Account;

[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly MessDbContext _context;
    private readonly IEmailService _emailService;

    public ChangePasswordModel(MessDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [BindProperty(SupportsGet = true)]
    public int Step { get; set; } = 1;
    
    public string? UserEmail { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    [BindProperty]
    public string? VerificationCode { get; set; }

    [BindProperty]
    public string? NewPassword { get; set; }

    [BindProperty]
    public string? ConfirmPassword { get; set; }

    private async Task<Models.User?> GetCurrentUserAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return null;

        return await _context.Users.Include(u => u.Member).FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        UserEmail = user.Email;

        return Page();
    }

    public async Task<IActionResult> OnPostSendCodeAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        UserEmail = user.Email;

        if (string.IsNullOrEmpty(user.Email))
        {
            ErrorMessage = "No email address is associated with your account.";
            Step = 1;
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
        await _emailService.SendPasswordResetCodeAsync(user.Email, userName, code);

        // Redirect to step 2
        return RedirectToPage(new { Step = 2 });
    }

    public async Task<IActionResult> OnPostVerifyCodeAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        UserEmail = user.Email;

        if (string.IsNullOrEmpty(VerificationCode))
        {
            ErrorMessage = "Please enter the verification code.";
            Step = 2;
            return Page();
        }

        // Verify the code
        var token = await _context.PasswordResetTokens
            .Where(t => t.UserId == user.Id && 
                       t.Code == VerificationCode && 
                       !t.IsUsed && 
                       t.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (token == null)
        {
            ErrorMessage = "Invalid or expired verification code. Please request a new one.";
            Step = 2;
            return Page();
        }

        // Mark token as used
        token.IsUsed = true;
        await _context.SaveChangesAsync();

        // Store verified state and redirect to step 3
        TempData["ChangePasswordVerified"] = user.Id;
        return RedirectToPage(new { Step = 3 });
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        UserEmail = user.Email;

        // Check if verification was completed (check TempData or allow if coming from valid flow)
        var verifiedUserId = TempData["ChangePasswordVerified"];
        if (verifiedUserId == null || (int)verifiedUserId != user.Id)
        {
            // Check if there's a recently used token for this user (within last 10 minutes)
            var recentToken = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && t.IsUsed && t.ExpiresAt > DateTime.UtcNow.AddMinutes(-10))
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (recentToken == null)
            {
                ErrorMessage = "Please complete email verification first.";
                Step = 1;
                return Page();
            }
        }

        // Validate password fields
        if (string.IsNullOrEmpty(NewPassword))
        {
            ErrorMessage = "New password is required.";
            Step = 3;
            TempData["ChangePasswordVerified"] = user.Id;
            return Page();
        }

        if (NewPassword.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters.";
            Step = 3;
            TempData["ChangePasswordVerified"] = user.Id;
            return Page();
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            Step = 3;
            TempData["ChangePasswordVerified"] = user.Id;
            return Page();
        }

        // Update password
        var (hash, salt) = PasswordHelper.HashPassword(NewPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        
        await _context.SaveChangesAsync();

        SuccessMessage = "Your password has been changed successfully!";
        Step = 3;

        return Page();
    }
}

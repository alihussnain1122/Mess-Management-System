using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using System.ComponentModel.DataAnnotations;

namespace MessManagement.Pages.Account
{
    public class VerifyResetCodeModel : PageModel
    {
        private readonly MessDbContext _context;

        public VerifyResetCodeModel(MessDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Verification code is required")]
            [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 digits")]
            public string Code { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            // Get email from TempData
            if (TempData["ResetEmail"] != null)
            {
                Email = TempData["ResetEmail"]!.ToString()!;
                TempData.Keep("ResetEmail");
            }

            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToPage("ForgotPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToPage("ForgotPassword");
            }

            // Find the user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                ErrorMessage = "Invalid request. Please try again.";
                return Page();
            }

            // Find valid reset token
            var token = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && 
                           t.Code == Input.Code && 
                           !t.IsUsed && 
                           t.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (token == null)
            {
                ErrorMessage = "Invalid or expired verification code. Please request a new one.";
                return Page();
            }

            // Mark token as used
            token.IsUsed = true;
            await _context.SaveChangesAsync();

            // Store verified email for reset password page
            TempData["VerifiedEmail"] = Email;
            TempData["ResetTokenId"] = token.Id;

            return RedirectToPage("ResetPassword");
        }
    }
}

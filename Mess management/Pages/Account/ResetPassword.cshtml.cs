using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MessManagement.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly MessDbContext _context;

        public ResetPasswordModel(MessDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "New password is required")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
                ErrorMessage = "Password must contain uppercase, lowercase, and number")]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please confirm your password")]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            // Get verified email from TempData
            if (TempData["VerifiedEmail"] != null)
            {
                Email = TempData["VerifiedEmail"]!.ToString()!;
                TempData.Keep("VerifiedEmail");
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

            // Update password
            var (hash, salt) = PasswordHelper.HashPassword(Input.NewPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            
            await _context.SaveChangesAsync();

            SuccessMessage = "Your password has been reset successfully!";
            
            // Clear TempData
            TempData.Remove("VerifiedEmail");
            TempData.Remove("ResetTokenId");

            return Page();
        }
    }
}

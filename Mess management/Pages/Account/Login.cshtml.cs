using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.ViewModels;
using MessManagement.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Pages.Account;

/// <summary>
/// Login page with account lockout protection.
/// After 5 failed attempts, account is locked for 15 minutes.
/// </summary>
[AllowAnonymous] // Explicitly allow unauthenticated access to login page
public class LoginModel : PageModel
{
    private readonly IUserService _userService;

    public LoginModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? ReturnUrl { get; set; }
    
    /// <summary>
    /// Indicates if the account is currently locked out.
    /// </summary>
    public bool IsLockedOut { get; set; }
    
    /// <summary>
    /// Time remaining until lockout expires.
    /// </summary>
    public TimeSpan? LockoutTimeRemaining { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Check if account is locked before attempting authentication
        var (isLocked, lockoutEnd) = await _userService.IsAccountLockedAsync(Input.Username);
        if (isLocked && lockoutEnd.HasValue)
        {
            IsLockedOut = true;
            LockoutTimeRemaining = lockoutEnd.Value - DateTime.UtcNow;
            ErrorMessage = $"Account is locked. Please try again in {Math.Ceiling(LockoutTimeRemaining.Value.TotalMinutes)} minutes.";
            return Page();
        }

        var user = await _userService.AuthenticateAsync(Input.Username, Input.Password);

        if (user == null)
        {
            // Check if account got locked after this failed attempt
            var (nowLocked, newLockoutEnd) = await _userService.IsAccountLockedAsync(Input.Username);
            if (nowLocked && newLockoutEnd.HasValue)
            {
                IsLockedOut = true;
                LockoutTimeRemaining = newLockoutEnd.Value - DateTime.UtcNow;
                ErrorMessage = $"Too many failed attempts. Account locked for {Math.Ceiling(LockoutTimeRemaining.Value.TotalMinutes)} minutes.";
            }
            else
            {
                ErrorMessage = "Invalid username or password";
            }
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = Input.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Redirect based on role
        if (user.Role == Models.UserRole.Admin)
        {
            return RedirectToPage("/Dashboard", new { area = "Admin" });
        }

        return RedirectToPage("/Dashboard", new { area = "User" });
    }
}

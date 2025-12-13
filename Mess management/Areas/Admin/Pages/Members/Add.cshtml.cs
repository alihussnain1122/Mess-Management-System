using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Members;

[Authorize(Roles = "Admin")]
public class AddModel : PageModel
{
    private readonly IMemberService _memberService;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    public AddModel(IMemberService memberService, IUserService userService, IEmailService emailService)
    {
        _memberService = memberService;
        _userService = userService;
        _emailService = emailService;
    }

    [BindProperty]
    public AddMemberViewModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userService.CreateUserAsync(Input.Username, Input.Password, UserRole.User, Input.Email);

        var member = new Member
        {
            FullName = Input.FullName,
            RoomNumber = Input.RoomNumber,
            UserId = user.Id
        };

        await _memberService.AddMemberAsync(member);

        // Send welcome email if email is configured
        if (!string.IsNullOrEmpty(Input.Email))
        {
            await _emailService.SendWelcomeEmailAsync(Input.Email, Input.FullName, Input.Username);
        }

        TempData["ToastSuccess"] = $"Member {Input.FullName} added successfully!";
        return RedirectToPage("Index");
    }
}
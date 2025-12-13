using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Members;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly IMemberService _memberService;

    public EditModel(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [BindProperty]
    public EditMemberViewModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();

        Input = new EditMemberViewModel
        {
            MemberId = member.MemberId,
            FullName = member.FullName,
            RoomNumber = member.RoomNumber,
            IsActive = member.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var member = await _memberService.GetMemberByIdAsync(Input.MemberId);
        if (member == null)
            return NotFound();

        member.FullName = Input.FullName;
        member.RoomNumber = Input.RoomNumber;
        member.IsActive = Input.IsActive;

        await _memberService.UpdateMemberAsync(member);

        return RedirectToPage("Index");
    }
}
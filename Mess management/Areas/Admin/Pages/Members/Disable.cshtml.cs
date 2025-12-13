using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Members;

[Authorize(Roles = "Admin")]
public class DisableModel : PageModel
{
    private readonly IMemberService _memberService;

    public DisableModel(IMemberService memberService)
    {
        _memberService = memberService;
    }

    public string MemberName { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();

        MemberName = member.FullName;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _memberService.DisableMemberAsync(id);
        return RedirectToPage("Index");
    }
}
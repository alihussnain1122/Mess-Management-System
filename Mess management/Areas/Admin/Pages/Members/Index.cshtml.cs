using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Members;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IMemberService _memberService;
    private const int PageSize = 10;

    public IndexModel(IMemberService memberService)
    {
        _memberService = memberService;
    }

    public PaginatedList<Member>? Members { get; set; }
    public string? SearchQuery { get; set; }

    public async Task OnGetAsync(string? search, int pageIndex = 1)
    {
        SearchQuery = search;
        var allMembers = await _memberService.GetAllMembersAsync();
        
        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            allMembers = allMembers.Where(m => 
                m.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                m.RoomNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
            );
        }
        
        Members = PaginatedList<Member>.Create(allMembers, pageIndex, PageSize, search);
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member != null)
        {
            member.IsActive = !member.IsActive;
            await _memberService.UpdateMemberAsync(member);
            TempData["Success"] = $"Member {(member.IsActive ? "activated" : "deactivated")} successfully!";
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _memberService.DeleteMemberAsync(id);
        if (result)
            TempData["Success"] = "Member deleted successfully!";
        else
            TempData["Error"] = "Failed to delete member. They may have associated records.";
        
        return RedirectToPage();
    }
}
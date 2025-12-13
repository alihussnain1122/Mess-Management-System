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
}
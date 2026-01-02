using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.ViewModels;
using MessManagement.Data;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly IReportService _reportService;
    private readonly MessDbContext _context;

    public DashboardModel(IReportService reportService, MessDbContext context)
    {
        _reportService = reportService;
        _context = context;
    }

    public DashboardSummary Summary { get; set; } = new();
    public int PendingSuggestionsCount { get; set; }

    public async Task OnGetAsync()
    {
        Summary = await _reportService.GetDashboardSummaryAsync();
        PendingSuggestionsCount = await _context.Suggestions
            .CountAsync(s => s.Status == SuggestionStatus.Pending);
    }
}
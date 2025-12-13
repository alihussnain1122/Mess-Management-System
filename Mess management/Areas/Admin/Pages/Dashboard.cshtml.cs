using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly IReportService _reportService;

    public DashboardModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    public DashboardSummary Summary { get; set; } = new();

    public async Task OnGetAsync()
    {
        Summary = await _reportService.GetDashboardSummaryAsync();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace MessManagement.Areas.Admin.Pages.Reports;

[Authorize(Roles = "Admin")]
public class MemberSummaryModel : PageModel
{
    private readonly IReportService _reportService;

    public MemberSummaryModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    public int SelectedMonth { get; set; } = DateTime.Now.Month;
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public IEnumerable<MemberCostBreakdown> MemberBreakdowns { get; set; } = new List<MemberCostBreakdown>();

    public async Task OnGetAsync(int? month, int? year)
    {
        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;
        MemberBreakdowns = await _reportService.GetAllMembersCostBreakdownAsync(SelectedMonth, SelectedYear);
    }

    public async Task<IActionResult> OnGetDownloadAsync(int? month, int? year)
    {
        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;
        MemberBreakdowns = await _reportService.GetAllMembersCostBreakdownAsync(SelectedMonth, SelectedYear);

        var monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth);

        var csv = new StringBuilder();
        csv.AppendLine($"Member Cost Summary - {monthName} {SelectedYear}");
        csv.AppendLine();
        csv.AppendLine("Member Name,Room Number,Present Days,Meal Cost,Total Cost,Total Paid,Balance Due");
        
        foreach (var member in MemberBreakdowns)
        {
            csv.AppendLine($"{member.MemberName},{member.RoomNumber},{member.TotalPresentDays},Rs.{member.MealCost:N2},Rs.{member.TotalCost:N2},Rs.{member.TotalPaid:N2},Rs.{member.BalanceDue:N2}");
        }

        csv.AppendLine();
        csv.AppendLine($"TOTAL,,{MemberBreakdowns.Sum(m => m.TotalPresentDays)},Rs.{MemberBreakdowns.Sum(m => m.MealCost):N2},Rs.{MemberBreakdowns.Sum(m => m.TotalCost):N2},Rs.{MemberBreakdowns.Sum(m => m.TotalPaid):N2},Rs.{MemberBreakdowns.Sum(m => m.BalanceDue):N2}");

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"MemberSummary_{SelectedYear}-{SelectedMonth:D2}.csv");
    }
}
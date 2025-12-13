using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace MessManagement.Areas.Admin.Pages.Reports;

[Authorize(Roles = "Admin")]
public class MonthlyModel : PageModel
{
    private readonly IReportService _reportService;
    private readonly IMemberService _memberService;
    private readonly IAttendanceService _attendanceService;
    private readonly IPaymentService _paymentService;
    private readonly IPdfService _pdfService;

    public MonthlyModel(IReportService reportService, IMemberService memberService, 
        IAttendanceService attendanceService, IPaymentService paymentService, IPdfService pdfService)
    {
        _reportService = reportService;
        _memberService = memberService;
        _attendanceService = attendanceService;
        _paymentService = paymentService;
        _pdfService = pdfService;
    }

    public int SelectedMonth { get; set; } = DateTime.Now.Month;
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public MonthlyReport Report { get; set; } = new();

    public async Task OnGetAsync(int? month, int? year)
    {
        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;
        Report = await _reportService.GetMonthlyReportAsync(SelectedMonth, SelectedYear);
    }

    public async Task<IActionResult> OnGetDownloadAsync(int? month, int? year)
    {
        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;
        Report = await _reportService.GetMonthlyReportAsync(SelectedMonth, SelectedYear);

        var monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth);

        var csv = new StringBuilder();
        csv.AppendLine($"Monthly Report - {monthName} {SelectedYear}");
        csv.AppendLine();
        csv.AppendLine("Summary");
        csv.AppendLine("Active Members," + Report.TotalActiveMembers);
        csv.AppendLine("Total Present Days," + Report.TotalPresentDays);
        csv.AppendLine("Total Absent Days," + Report.TotalAbsentDays);
        csv.AppendLine("Total Revenue,Rs." + Report.TotalRevenue.ToString("N2"));
        csv.AppendLine("Average Daily Cost,Rs." + Report.AverageDailyCost.ToString("N2"));
        csv.AppendLine("Expected Revenue,Rs." + Report.ExpectedRevenue.ToString("N2"));

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"MonthlyReport_{SelectedYear}-{SelectedMonth:D2}.csv");
    }

    public async Task<IActionResult> OnGetDownloadPdfAsync(int? month, int? year)
    {
        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;
        Report = await _reportService.GetMonthlyReportAsync(SelectedMonth, SelectedYear);

        var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
        var members = await _memberService.GetActiveMembersAsync();

        var memberSummaries = new List<MemberSummaryItem>();
        foreach (var member in members)
        {
            var presentDays = await _attendanceService.GetPresentCountForMemberAsync(member.MemberId, SelectedMonth, SelectedYear);
            var totalPaid = await _paymentService.GetTotalPaymentsForPeriodAsync(member.MemberId, startDate, endDate);
            
            memberSummaries.Add(new MemberSummaryItem
            {
                MemberName = member.FullName,
                DaysPresent = presentDays,
                TotalPaid = totalPaid,
                Balance = 0 // Calculate if needed
            });
        }

        var pdfReport = new MonthlyReportViewModel
        {
            Month = startDate,
            TotalRevenue = Report.TotalRevenue,
            CashCollection = payments.Where(p => p.PaymentMode == MessManagement.Models.PaymentMode.Cash && p.Status == MessManagement.Models.PaymentStatus.Completed).Sum(p => p.Amount),
            OnlineCollection = payments.Where(p => p.PaymentMode == MessManagement.Models.PaymentMode.Online && p.Status == MessManagement.Models.PaymentStatus.Completed).Sum(p => p.Amount),
            TotalMembers = Report.TotalActiveMembers,
            AverageAttendance = Report.TotalPresentDays > 0 ? (double)Report.TotalPresentDays / (Report.TotalPresentDays + Report.TotalAbsentDays) * 100 : 0,
            WorkingDays = DateTime.DaysInMonth(SelectedYear, SelectedMonth),
            MemberSummaries = memberSummaries
        };

        var pdfBytes = _pdfService.GenerateMonthlyReport(pdfReport);
        var monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth);
        return File(pdfBytes, "application/pdf", $"MonthlyReport_{monthName}_{SelectedYear}.pdf");
    }
}
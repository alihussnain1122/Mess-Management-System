using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace MessManagement.Areas.Admin.Pages.Reports;

[Authorize(Roles = "Admin")]
public class DailyModel : PageModel
{
    private readonly IReportService _reportService;
    private readonly IAttendanceService _attendanceService;
    private readonly IMemberService _memberService;
    private readonly IPaymentService _paymentService;
    private readonly IPdfService _pdfService;

    public DailyModel(IReportService reportService, IAttendanceService attendanceService, 
        IMemberService memberService, IPaymentService paymentService, IPdfService pdfService)
    {
        _reportService = reportService;
        _attendanceService = attendanceService;
        _memberService = memberService;
        _paymentService = paymentService;
        _pdfService = pdfService;
    }

    public DateTime SelectedDate { get; set; } = DateTime.Today;
    public DailyReport Report { get; set; } = new();

    public async Task OnGetAsync(DateTime? date)
    {
        SelectedDate = date ?? DateTime.Today;
        Report = await _reportService.GetDailyReportAsync(SelectedDate);
    }

    public async Task<IActionResult> OnGetDownloadAsync(DateTime? date)
    {
        SelectedDate = date ?? DateTime.Today;
        Report = await _reportService.GetDailyReportAsync(SelectedDate);

        var csv = new StringBuilder();
        csv.AppendLine("Daily Report - " + SelectedDate.ToString("MMMM dd, yyyy"));
        csv.AppendLine();
        csv.AppendLine("Summary");
        csv.AppendLine("Total Members," + Report.TotalMembers);
        csv.AppendLine("Present," + Report.PresentCount);
        csv.AppendLine("Absent," + Report.AbsentCount);
        csv.AppendLine("Payments Received,Rs." + Report.TotalPaymentsReceived.ToString("N2"));
        csv.AppendLine();
        csv.AppendLine("Menu Items");
        csv.AppendLine("Meal Type,Dish Name,Price");
        foreach (var item in Report.MenuItems)
        {
            csv.AppendLine($"{item.MealType},{item.DishName},Rs.{item.Price:N2}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"DailyReport_{SelectedDate:yyyy-MM-dd}.csv");
    }

    public async Task<IActionResult> OnGetDownloadPdfAsync(DateTime? date)
    {
        SelectedDate = date ?? DateTime.Today;
        Report = await _reportService.GetDailyReportAsync(SelectedDate);

        // Get attendance details
        var attendance = await _attendanceService.GetAttendanceForDateAsync(SelectedDate);
        var members = await _memberService.GetActiveMembersAsync();
        var payments = await _paymentService.GetPaymentsForDateAsync(SelectedDate);

        var pdfReport = new DailyReportViewModel
        {
            Date = SelectedDate,
            TotalMembers = Report.TotalMembers,
            PresentCount = Report.PresentCount,
            AbsentCount = Report.AbsentCount,
            TotalCollection = payments.Where(p => p.Status == MessManagement.Models.PaymentStatus.Completed).Sum(p => p.Amount),
            CashCollection = payments.Where(p => p.PaymentMode == MessManagement.Models.PaymentMode.Cash && p.Status == MessManagement.Models.PaymentStatus.Completed).Sum(p => p.Amount),
            OnlineCollection = payments.Where(p => p.PaymentMode == MessManagement.Models.PaymentMode.Online && p.Status == MessManagement.Models.PaymentStatus.Completed).Sum(p => p.Amount),
            AttendanceDetails = members.Select(m =>
            {
                var att = attendance.FirstOrDefault(a => a.MemberId == m.MemberId);
                return new AttendanceDetailItem
                {
                    MemberName = m.FullName,
                    RoomNumber = m.RoomNumber,
                    Status = att?.Status.ToString() ?? "Not Marked"
                };
            }).ToList()
        };

        var pdfBytes = _pdfService.GenerateDailyReport(pdfReport);
        return File(pdfBytes, "application/pdf", $"DailyReport_{SelectedDate:yyyy-MM-dd}.pdf");
    }
}
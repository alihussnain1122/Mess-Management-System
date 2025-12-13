using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Attendance;

[Authorize(Roles = "Admin")]
public class HistoryModel : PageModel
{
    private readonly IAttendanceService _attendanceService;
    private readonly IPdfService _pdfService;

    public HistoryModel(IAttendanceService attendanceService, IPdfService pdfService)
    {
        _attendanceService = attendanceService;
        _pdfService = pdfService;
    }

    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);
    public DateTime EndDate { get; set; } = DateTime.Today;
    public IEnumerable<MessManagement.Models.Attendance>? AttendanceRecords { get; set; }
    
    // Summary statistics
    public int TotalBreakfasts { get; set; }
    public int TotalLunches { get; set; }
    public int TotalDinners { get; set; }
    public int TotalMeals { get; set; }

    public async Task OnGetAsync(DateTime? startDate, DateTime? endDate)
    {
        StartDate = startDate ?? DateTime.Today.AddDays(-7);
        EndDate = endDate ?? DateTime.Today;

        var records = new List<MessManagement.Models.Attendance>();
        for (var date = StartDate; date <= EndDate; date = date.AddDays(1))
        {
            var dayRecords = await _attendanceService.GetAttendanceForDateAsync(date);
            records.AddRange(dayRecords);
        }

        AttendanceRecords = records.OrderByDescending(r => r.Date).ThenBy(r => r.Member?.FullName);
        
        // Calculate summary
        TotalBreakfasts = records.Count(r => r.BreakfastPresent);
        TotalLunches = records.Count(r => r.LunchPresent);
        TotalDinners = records.Count(r => r.DinnerPresent);
        TotalMeals = TotalBreakfasts + TotalLunches + TotalDinners;
    }

    public async Task<IActionResult> OnPostDownloadPdfAsync(DateTime startDate, DateTime endDate)
    {
        var records = new List<MessManagement.Models.Attendance>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayRecords = await _attendanceService.GetAttendanceForDateAsync(date);
            records.AddRange(dayRecords);
        }

        var orderedRecords = records.OrderByDescending(r => r.Date).ThenBy(r => r.Member?.FullName).ToList();

        var sheet = new AttendanceSheetViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            Title = $"Attendance Sheet ({startDate:dd MMM} - {endDate:dd MMM yyyy})",
            TotalMembers = orderedRecords.Select(r => r.MemberId).Distinct().Count(),
            TotalBreakfasts = orderedRecords.Count(r => r.BreakfastPresent),
            TotalLunches = orderedRecords.Count(r => r.LunchPresent),
            TotalDinners = orderedRecords.Count(r => r.DinnerPresent),
            TotalMeals = orderedRecords.Sum(r => r.MealsAttended),
            Records = orderedRecords.Select(r => new AttendanceSheetRecord
            {
                Date = r.Date,
                MemberName = r.Member?.FullName ?? "Unknown",
                RoomNumber = r.Member?.RoomNumber ?? "N/A",
                BreakfastPresent = r.BreakfastPresent,
                LunchPresent = r.LunchPresent,
                DinnerPresent = r.DinnerPresent,
                MealsAttended = r.MealsAttended,
                MarkedBy = r.MarkedByUser?.Username ?? "Admin"
            }).ToList()
        };

        var pdfBytes = _pdfService.GenerateAttendanceSheet(sheet);
        var fileName = $"Attendance_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}
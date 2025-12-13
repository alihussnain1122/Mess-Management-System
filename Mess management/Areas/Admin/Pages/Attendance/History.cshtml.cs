using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Attendance;

[Authorize(Roles = "Admin")]
public class HistoryModel : PageModel
{
    private readonly IAttendanceService _attendanceService;

    public HistoryModel(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);
    public DateTime EndDate { get; set; } = DateTime.Today;
    public IEnumerable<MessManagement.Models.Attendance>? AttendanceRecords { get; set; }

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
    }
}
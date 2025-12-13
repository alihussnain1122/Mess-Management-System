using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MessManagement.Areas.User.Pages.Attendance;

[Authorize(Roles = "User")]
public class ViewModel : PageModel
{
    private readonly IAttendanceService _attendanceService;
    private readonly IMemberService _memberService;

    public ViewModel(IAttendanceService attendanceService, IMemberService memberService)
    {
        _attendanceService = attendanceService;
        _memberService = memberService;
    }

    public int SelectedMonth { get; set; } = DateTime.Now.Month;
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public IEnumerable<MessManagement.Models.Attendance> AttendanceRecords { get; set; } = new List<MessManagement.Models.Attendance>();
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendanceRate { get; set; }

    public async Task OnGetAsync(int? month, int? year)
    {
        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return;

        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member == null) return;

        var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        AttendanceRecords = await _attendanceService.GetAttendanceForMemberAsync(member.MemberId, startDate, endDate);
        
        PresentCount = AttendanceRecords.Count(a => a.Status == AttendanceStatus.Present);
        AbsentCount = AttendanceRecords.Count(a => a.Status == AttendanceStatus.Absent);
        
        var total = PresentCount + AbsentCount;
        AttendanceRate = total > 0 ? (double)PresentCount / total * 100 : 0;
    }
}
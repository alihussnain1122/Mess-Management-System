using MessManagement.Models;

namespace MessManagement.ViewModels;

public class MonthlyAttendanceReport
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public decimal AttendancePercentage => TotalDays > 0 ? (decimal)PresentDays / TotalDays * 100 : 0;
    public IEnumerable<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}

public class MealWiseAttendanceSummary
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalDays { get; set; }
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    public int TotalMeals => BreakfastCount + LunchCount + DinnerCount;
    public IEnumerable<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}

public class MealCounts
{
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    public int TotalMeals => BreakfastCount + LunchCount + DinnerCount;
}

public class AttendanceMismatch
{
    public int MemberId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus UserReportedStatus { get; set; }
    public AttendanceStatus AdminMarkedStatus { get; set; }
    public string? Notes { get; set; }
}

public class AttendanceMarkViewModel
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public bool BreakfastPresent { get; set; } = true;
    public bool LunchPresent { get; set; } = true;
    public bool DinnerPresent { get; set; } = true;
    
    // Legacy property for compatibility
    public bool IsPresent => BreakfastPresent || LunchPresent || DinnerPresent;
}

public class DailyAttendanceViewModel
{
    public DateTime Date { get; set; }
    public List<AttendanceMarkViewModel> Members { get; set; } = new();
}

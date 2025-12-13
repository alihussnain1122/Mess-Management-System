namespace MessManagement.ViewModels;

public class DailyReport
{
    public DateTime Date { get; set; }
    public int TotalMembers { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    public decimal TotalPaymentsReceived { get; set; }
    public int PaymentCount { get; set; }
    public List<MenuItemViewModel> MenuItems { get; set; } = new();
}

public class MonthlyReport
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalActiveMembers { get; set; }
    public int TotalPresentDays { get; set; }
    public int TotalAbsentDays { get; set; }
    public int TotalBreakfasts { get; set; }
    public int TotalLunches { get; set; }
    public int TotalDinners { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal ExpectedRevenue { get; set; }
    public decimal AverageDailyCost { get; set; }
    public decimal CollectionEfficiency => ExpectedRevenue > 0 ? (TotalRevenue / ExpectedRevenue) * 100 : 0;
}

public class MemberCostBreakdown
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalPresentDays { get; set; }
    public int TotalAbsentDays { get; set; }
    
    // Meal-wise attendance
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    
    // Meal-wise costs
    public decimal BreakfastCost { get; set; }
    public decimal LunchCost { get; set; }
    public decimal DinnerCost { get; set; }
    
    // Total calculations
    public decimal MealCost { get; set; }
    public decimal WaterTeaCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
}

public class WeeklyMenuSummary
{
    public List<DailyMenuSummary> DailySummaries { get; set; } = new();
    public decimal TotalWeeklyCost { get; set; }
    public decimal AverageDailyCost { get; set; }
}

public class DailyMenuSummary
{
    public DayOfWeek DayOfWeek { get; set; }
    public List<MenuItemViewModel> Items { get; set; } = new();
    public decimal TotalCost { get; set; }
}

public class MenuItemViewModel
{
    public int Id { get; set; }
    public string DishName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string MealType { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
}

public class DashboardSummary
{
    public int TotalMembers { get; set; }
    public int ActiveMembers { get; set; }
    public int TodayPresentCount { get; set; }
    public int TodayAbsentCount { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal MonthlyExpectedRevenue { get; set; }
    public int CurrentMonth { get; set; }
    public int CurrentYear { get; set; }
    public List<DailyAttendanceStat> WeeklyAttendance { get; set; } = new();
    public List<DailyRevenueStat> WeeklyRevenue { get; set; } = new();
}

public class DailyAttendanceStat
{
    public string Day { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
}

public class DailyRevenueStat
{
    public string Day { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

// PDF Report View Models
public class DailyReportViewModel
{
    public DateTime Date { get; set; }
    public int TotalMembers { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public decimal TotalCollection { get; set; }
    public decimal CashCollection { get; set; }
    public decimal OnlineCollection { get; set; }
    public List<AttendanceDetailItem> AttendanceDetails { get; set; } = new();
}

public class AttendanceDetailItem
{
    public string MemberName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public string Status { get; set; } = "";
}

public class WeeklyReportViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCollection { get; set; }
    public double AverageAttendance { get; set; }
    public int TotalPayments { get; set; }
    public List<DailyBreakdownItem> DailyBreakdown { get; set; } = new();
}

public class DailyBreakdownItem
{
    public DateTime Date { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public decimal Collection { get; set; }
}

public class MonthlyReportViewModel
{
    public DateTime Month { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal CashCollection { get; set; }
    public decimal OnlineCollection { get; set; }
    public int TotalMembers { get; set; }
    public double AverageAttendance { get; set; }
    public int WorkingDays { get; set; }
    public List<MemberSummaryItem> MemberSummaries { get; set; } = new();
}

public class MemberSummaryItem
{
    public string MemberName { get; set; } = "";
    public int DaysPresent { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
}


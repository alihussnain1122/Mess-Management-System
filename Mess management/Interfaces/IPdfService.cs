using MessManagement.ViewModels;

namespace MessManagement.Interfaces;

public interface IPdfService
{
    byte[] GenerateDailyReport(DailyReportViewModel report);
    byte[] GenerateWeeklyReport(WeeklyReportViewModel report);
    byte[] GenerateMonthlyReport(MonthlyReportViewModel report);
    byte[] GenerateMemberStatement(string memberName, IEnumerable<PaymentStatementItem> payments, decimal totalAmount);
    byte[] GenerateMemberMonthlyBill(MemberMonthlyBillViewModel bill);
    byte[] GenerateAttendanceSheet(AttendanceSheetViewModel sheet);
}

public class AttendanceSheetViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Title { get; set; } = "Attendance Sheet";
    public List<AttendanceSheetRecord> Records { get; set; } = new();
    
    // Summary
    public int TotalMembers { get; set; }
    public int TotalBreakfasts { get; set; }
    public int TotalLunches { get; set; }
    public int TotalDinners { get; set; }
    public int TotalMeals { get; set; }
}

public class AttendanceSheetRecord
{
    public DateTime Date { get; set; }
    public string MemberName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public bool BreakfastPresent { get; set; }
    public bool LunchPresent { get; set; }
    public bool DinnerPresent { get; set; }
    public int MealsAttended { get; set; }
    public string MarkedBy { get; set; } = "";
}

public class PaymentStatementItem
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public string Status { get; set; } = "";
}

public class MemberMonthlyBillViewModel
{
    public string MemberName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public int Month { get; set; }
    public int Year { get; set; }
    public string MonthName { get; set; } = "";
    
    // Attendance Summary
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    
    // Meal-wise Attendance Counts
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    
    // Meal-wise Rates
    public decimal BreakfastRate { get; set; }
    public decimal LunchRate { get; set; }
    public decimal DinnerRate { get; set; }
    
    // Meal-wise Charges
    public decimal BreakfastCharges { get; set; }
    public decimal LunchCharges { get; set; }
    public decimal DinnerCharges { get; set; }
    
    // Legacy - Total Meal Charges
    public decimal MealRate { get; set; }
    public decimal TotalMealCharges { get; set; }
    
    // Additional Charges
    public int WaterCount { get; set; }
    public decimal WaterRate { get; set; }
    public decimal WaterCharges { get; set; }
    
    public int TeaCount { get; set; }
    public decimal TeaRate { get; set; }
    public decimal TeaCharges { get; set; }
    
    // Totals
    public decimal GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    
    // Payment History
    public List<PaymentStatementItem> Payments { get; set; } = new();
    
    // Daily Attendance Breakdown
    public List<DailyAttendanceItem> DailyAttendance { get; set; } = new();
}

public class DailyAttendanceItem
{
    public DateTime Date { get; set; }
    public string Status { get; set; } = "";
    public bool Breakfast { get; set; }
    public bool Lunch { get; set; }
    public bool Dinner { get; set; }
}

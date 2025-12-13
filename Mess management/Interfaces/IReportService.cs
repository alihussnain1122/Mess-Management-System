using MessManagement.ViewModels;

namespace MessManagement.Interfaces;

public interface IReportService
{
    Task<DailyReport> GetDailyReportAsync(DateTime date);
    Task<MonthlyReport> GetMonthlyReportAsync(int month, int year);
    Task<MemberCostBreakdown> GetMemberCostBreakdownAsync(int memberId, int month, int year);
    Task<IEnumerable<MemberCostBreakdown>> GetAllMembersCostBreakdownAsync(int month, int year);
    Task<WeeklyMenuSummary> GetWeeklyMenuSummaryAsync();
    Task<DashboardSummary> GetDashboardSummaryAsync();
    Task<IEnumerable<PaymentSummary>> GetPaymentReportAsync(DateTime startDate, DateTime endDate);
}

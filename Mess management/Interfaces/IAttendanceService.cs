using MessManagement.Models;
using MessManagement.ViewModels;

namespace MessManagement.Interfaces;

public interface IAttendanceService
{
    Task<IEnumerable<Attendance>> GetAttendanceForMemberAsync(int memberId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<Attendance>> GetAttendanceForDateAsync(DateTime date);
    Task<Attendance?> GetAttendanceByIdAsync(int id);
    Task<Attendance?> GetAttendanceAsync(int memberId, DateTime date);
    
    // New meal-wise attendance methods
    Task<Attendance> MarkAttendanceAsync(int memberId, DateTime date, bool breakfast, bool lunch, bool dinner, int markedBy);
    Task MarkAllAttendanceAsync(DateTime date, bool breakfast, bool lunch, bool dinner, int markedBy);
    
    // Legacy methods (for backward compatibility)
    Task<Attendance> MarkPresentAsync(int memberId, DateTime date, int markedBy);
    Task<Attendance> MarkAbsentAsync(int memberId, DateTime date, int markedBy);
    Task MarkAllPresentAsync(DateTime date, int markedBy);
    Task<Attendance> UpdateAttendanceAsync(int attendanceId, AttendanceStatus status, int markedBy);
    Task<bool> DeleteAttendanceAsync(int id);
    
    // Reports
    Task<MonthlyAttendanceReport> GetMonthlyAttendanceReportAsync(int memberId, int month, int year);
    Task<MealWiseAttendanceSummary> GetMealWiseAttendanceSummaryAsync(int memberId, int month, int year);
    Task<IEnumerable<AttendanceMismatch>> VerifyAttendanceMismatchAsync(int memberId, DateTime startDate, DateTime endDate);
    
    // Counts
    Task<int> GetPresentCountForMemberAsync(int memberId, int month, int year);
    Task<int> GetAbsentCountForMemberAsync(int memberId, int month, int year);
    Task<MealCounts> GetMealCountsForMemberAsync(int memberId, int month, int year);
}

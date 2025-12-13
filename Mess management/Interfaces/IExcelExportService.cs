namespace MessManagement.Interfaces;

public interface IExcelExportService
{
    Task<byte[]> ExportMembersAsync();
    Task<byte[]> ExportPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<byte[]> ExportAttendanceAsync(int month, int year);
    Task<byte[]> ExportMonthlyReportAsync(int month, int year);
}

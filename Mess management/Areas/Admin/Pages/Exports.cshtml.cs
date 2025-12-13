using MessManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MessManagement.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class ExportsModel : PageModel
{
    private readonly IExcelExportService _excelExportService;

    public ExportsModel(IExcelExportService excelExportService)
    {
        _excelExportService = excelExportService;
    }

    [BindProperty]
    public DateTime? PaymentStartDate { get; set; }
    
    [BindProperty]
    public DateTime? PaymentEndDate { get; set; }
    
    [BindProperty]
    public int AttendanceMonth { get; set; } = DateTime.Now.Month;
    
    [BindProperty]
    public int AttendanceYear { get; set; } = DateTime.Now.Year;
    
    [BindProperty]
    public int ReportMonth { get; set; } = DateTime.Now.Month;
    
    [BindProperty]
    public int ReportYear { get; set; } = DateTime.Now.Year;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostExportMembersAsync()
    {
        try
        {
            var fileBytes = await _excelExportService.ExportMembersAsync();
            var fileName = $"Members_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Export failed: {ex.Message}";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostExportPaymentsAsync()
    {
        try
        {
            var fileBytes = await _excelExportService.ExportPaymentsAsync(PaymentStartDate, PaymentEndDate);
            var dateRange = "";
            if (PaymentStartDate.HasValue && PaymentEndDate.HasValue)
                dateRange = $"_{PaymentStartDate.Value:yyyyMMdd}_to_{PaymentEndDate.Value:yyyyMMdd}";
            else if (PaymentStartDate.HasValue)
                dateRange = $"_from_{PaymentStartDate.Value:yyyyMMdd}";
            else if (PaymentEndDate.HasValue)
                dateRange = $"_until_{PaymentEndDate.Value:yyyyMMdd}";
                
            var fileName = $"Payments{dateRange}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Export failed: {ex.Message}";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostExportAttendanceAsync()
    {
        try
        {
            var fileBytes = await _excelExportService.ExportAttendanceAsync(AttendanceMonth, AttendanceYear);
            var fileName = $"Attendance_{AttendanceYear}_{AttendanceMonth:00}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Export failed: {ex.Message}";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostExportMonthlyReportAsync()
    {
        try
        {
            var fileBytes = await _excelExportService.ExportMonthlyReportAsync(ReportMonth, ReportYear);
            var fileName = $"MonthlyReport_{ReportYear}_{ReportMonth:00}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Export failed: {ex.Message}";
            return Page();
        }
    }
}

using ClosedXML.Excel;
using MessManagement.Data;
using MessManagement.Helpers;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Services;

public class ExcelExportService : IExcelExportService
{
    private readonly MessDbContext _context;

    public ExcelExportService(MessDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportMembersAsync()
    {
        var members = await _context.Members
            .Include(m => m.User)
            .OrderBy(m => m.FullName)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Members");

        // Header styling
        var headerRange = worksheet.Range("A1:G1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Headers
        worksheet.Cell("A1").Value = "Member ID";
        worksheet.Cell("B1").Value = "Full Name";
        worksheet.Cell("C1").Value = "Room Number";
        worksheet.Cell("D1").Value = "Email";
        worksheet.Cell("E1").Value = "Phone";
        worksheet.Cell("F1").Value = "Join Date";
        worksheet.Cell("G1").Value = "Status";

        // Data
        int row = 2;
        foreach (var member in members)
        {
            worksheet.Cell(row, 1).Value = member.MemberId;
            worksheet.Cell(row, 2).Value = member.FullName;
            worksheet.Cell(row, 3).Value = member.RoomNumber;
            worksheet.Cell(row, 4).Value = member.User?.Email ?? "N/A";
            worksheet.Cell(row, 5).Value = "N/A"; // Phone not in User model
            worksheet.Cell(row, 6).Value = member.JoinDate.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 7).Value = member.IsActive ? "Active" : "Inactive";
            
            // Alternate row coloring
            if (row % 2 == 0)
            {
                worksheet.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F4F6");
            }
            
            row++;
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();
        
        // Add table borders
        var dataRange = worksheet.Range(1, 1, row - 1, 7);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Payments
            .Include(p => p.Member)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(p => p.Date >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(p => p.Date <= endDate.Value);

        var payments = await query.OrderByDescending(p => p.Date).ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Payments");

        // Header styling
        var headerRange = worksheet.Range("A1:H1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#059669");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Headers
        worksheet.Cell("A1").Value = "Payment ID";
        worksheet.Cell("B1").Value = "Member Name";
        worksheet.Cell("C1").Value = "Room No.";
        worksheet.Cell("D1").Value = "Amount (Rs)";
        worksheet.Cell("E1").Value = "Date";
        worksheet.Cell("F1").Value = "Payment Mode";
        worksheet.Cell("G1").Value = "Status";
        worksheet.Cell("H1").Value = "Notes";

        // Data
        int row = 2;
        decimal totalAmount = 0;
        foreach (var payment in payments)
        {
            worksheet.Cell(row, 1).Value = payment.Id;
            worksheet.Cell(row, 2).Value = payment.Member.FullName;
            worksheet.Cell(row, 3).Value = payment.Member.RoomNumber;
            worksheet.Cell(row, 4).Value = payment.Amount;
            worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 5).Value = payment.Date.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 6).Value = payment.PaymentMode.ToString();
            worksheet.Cell(row, 7).Value = payment.Status.ToString();
            worksheet.Cell(row, 8).Value = payment.Notes ?? "";
            
            totalAmount += payment.Amount;
            
            // Alternate row coloring
            if (row % 2 == 0)
            {
                worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F4F6");
            }
            
            row++;
        }

        // Total row
        worksheet.Cell(row, 3).Value = "Total:";
        worksheet.Cell(row, 3).Style.Font.Bold = true;
        worksheet.Cell(row, 4).Value = totalAmount;
        worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(row, 4).Style.Font.Bold = true;
        worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#D1FAE5");

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();
        
        // Add table borders
        var dataRange = worksheet.Range(1, 1, row, 8);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportAttendanceAsync(int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var members = await _context.Members
            .Where(m => m.IsActive)
            .OrderBy(m => m.FullName)
            .ToListAsync();

        var attendances = await _context.Attendances
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add($"Attendance {month:00}-{year}");

        // Headers
        worksheet.Cell("A1").Value = "Member Name";
        worksheet.Cell("B1").Value = "Room";
        
        // Day columns
        for (int day = 1; day <= daysInMonth; day++)
        {
            worksheet.Cell(1, day + 2).Value = day;
        }
        
        worksheet.Cell(1, daysInMonth + 3).Value = "Present";
        worksheet.Cell(1, daysInMonth + 4).Value = "Absent";
        worksheet.Cell(1, daysInMonth + 5).Value = "Attendance %";

        // Header styling
        var headerRange = worksheet.Range(1, 1, 1, daysInMonth + 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#7C3AED");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Data
        int row = 2;
        foreach (var member in members)
        {
            worksheet.Cell(row, 1).Value = member.FullName;
            worksheet.Cell(row, 2).Value = member.RoomNumber;
            
            int presentCount = 0;
            int absentCount = 0;
            
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                var attendance = attendances.FirstOrDefault(a => 
                    a.MemberId == member.MemberId && a.Date.Date == date.Date);
                
                if (attendance != null)
                {
                    if (attendance.Status == AttendanceStatus.Present)
                    {
                        worksheet.Cell(row, day + 2).Value = "P";
                        worksheet.Cell(row, day + 2).Style.Font.FontColor = XLColor.FromHtml("#059669");
                        presentCount++;
                    }
                    else
                    {
                        worksheet.Cell(row, day + 2).Value = "A";
                        worksheet.Cell(row, day + 2).Style.Font.FontColor = XLColor.FromHtml("#DC2626");
                        absentCount++;
                    }
                }
                else
                {
                    worksheet.Cell(row, day + 2).Value = "-";
                    worksheet.Cell(row, day + 2).Style.Font.FontColor = XLColor.Gray;
                }
                
                worksheet.Cell(row, day + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            
            worksheet.Cell(row, daysInMonth + 3).Value = presentCount;
            worksheet.Cell(row, daysInMonth + 4).Value = absentCount;
            
            var totalMarked = presentCount + absentCount;
            var percentage = totalMarked > 0 ? (double)presentCount / totalMarked * 100 : 0;
            worksheet.Cell(row, daysInMonth + 5).Value = $"{percentage:F1}%";
            
            // Color code attendance percentage
            if (percentage >= 80)
                worksheet.Cell(row, daysInMonth + 5).Style.Font.FontColor = XLColor.FromHtml("#059669");
            else if (percentage >= 60)
                worksheet.Cell(row, daysInMonth + 5).Style.Font.FontColor = XLColor.FromHtml("#F59E0B");
            else
                worksheet.Cell(row, daysInMonth + 5).Style.Font.FontColor = XLColor.FromHtml("#DC2626");
            
            // Alternate row coloring
            if (row % 2 == 0)
            {
                worksheet.Range(row, 1, row, daysInMonth + 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F4F6");
            }
            
            row++;
        }

        // Auto-fit first two columns, keep day columns narrow
        worksheet.Column(1).AdjustToContents();
        worksheet.Column(2).AdjustToContents();
        for (int col = 3; col <= daysInMonth + 2; col++)
        {
            worksheet.Column(col).Width = 4;
        }
        worksheet.Column(daysInMonth + 3).AdjustToContents();
        worksheet.Column(daysInMonth + 4).AdjustToContents();
        worksheet.Column(daysInMonth + 5).AdjustToContents();
        
        // Add table borders
        var dataRange = worksheet.Range(1, 1, row - 1, daysInMonth + 5);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportMonthlyReportAsync(int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var members = await _context.Members
            .Where(m => m.IsActive)
            .Include(m => m.User)
            .OrderBy(m => m.FullName)
            .ToListAsync();

        var attendances = await _context.Attendances
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        var payments = await _context.Payments
            .Where(p => p.Date >= startDate && p.Date <= endDate)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add($"Report {month:00}-{year}");

        // Title
        worksheet.Cell("A1").Value = $"Monthly Report - {startDate:MMMM yyyy}";
        worksheet.Range("A1:M1").Merge();
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1E40AF");
        worksheet.Cell("A1").Style.Font.FontColor = XLColor.White;

        // Headers (meal-wise)
        var headers = new[] { "Member", "Room", "Breakfast", "Lunch", "Dinner", "Total Meals", "Meal Charges", "Water", "Tea Charges", "Total Charges", "Paid", "Balance", "Status" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(3, i + 1).Value = headers[i];
        }
        
        var headerRange = worksheet.Range(3, 1, 3, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Rates from Constants
        decimal breakfastRate = Constants.DefaultBreakfastRate;
        decimal lunchRate = Constants.DefaultLunchRate;
        decimal dinnerRate = Constants.DefaultDinnerRate;
        decimal waterRate = Constants.DefaultWaterCost;  // FREE
        decimal teaRate = Constants.DefaultTeaCost;       // Rs.100 per meal

        // Data
        int row = 4;
        decimal grandTotalCharges = 0;
        decimal grandTotalPaid = 0;
        
        foreach (var member in members)
        {
            var memberAttendances = attendances.Where(a => a.MemberId == member.MemberId).ToList();
            
            // Meal-wise counts
            var breakfastCount = memberAttendances.Count(a => a.BreakfastPresent);
            var lunchCount = memberAttendances.Count(a => a.LunchPresent);
            var dinnerCount = memberAttendances.Count(a => a.DinnerPresent);
            var totalMeals = breakfastCount + lunchCount + dinnerCount;

            // Water & Tea are auto-included with every meal
            var waterCount = totalMeals;  // FREE
            var teaCount = totalMeals;    // Rs.100 per meal
            
            var breakfastCharges = breakfastCount * breakfastRate;
            var lunchCharges = lunchCount * lunchRate;
            var dinnerCharges = dinnerCount * dinnerRate;
            var mealCharges = breakfastCharges + lunchCharges + dinnerCharges;
            var waterCharges = waterCount * waterRate;  // Rs.0
            var teaCharges = teaCount * teaRate;        // Rs.100 per meal
            var totalCharges = mealCharges + waterCharges + teaCharges;
            
            var memberPayments = payments.Where(p => p.MemberId == member.MemberId).Sum(p => p.Amount);
            var balance = totalCharges - memberPayments;
            
            worksheet.Cell(row, 1).Value = member.FullName;
            worksheet.Cell(row, 2).Value = member.RoomNumber;
            worksheet.Cell(row, 3).Value = breakfastCount;
            worksheet.Cell(row, 4).Value = lunchCount;
            worksheet.Cell(row, 5).Value = dinnerCount;
            worksheet.Cell(row, 6).Value = totalMeals;
            worksheet.Cell(row, 7).Value = mealCharges;
            worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 8).Value = "FREE";
            worksheet.Cell(row, 8).Style.Font.FontColor = XLColor.FromHtml("#059669");
            worksheet.Cell(row, 9).Value = teaCharges;
            worksheet.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 10).Value = totalCharges;
            worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 11).Value = memberPayments;
            worksheet.Cell(row, 11).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 12).Value = balance;
            worksheet.Cell(row, 12).Style.NumberFormat.Format = "#,##0.00";
            
            // Status based on balance
            if (balance <= 0)
            {
                worksheet.Cell(row, 13).Value = "Paid";
                worksheet.Cell(row, 13).Style.Font.FontColor = XLColor.FromHtml("#059669");
            }
            else
            {
                worksheet.Cell(row, 13).Value = "Due";
                worksheet.Cell(row, 13).Style.Font.FontColor = XLColor.FromHtml("#DC2626");
            }
            
            grandTotalCharges += totalCharges;
            grandTotalPaid += memberPayments;
            
            // Alternate row coloring
            if (row % 2 == 0)
            {
                worksheet.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F4F6");
            }
            
            row++;
        }

        // Total row
        worksheet.Cell(row, 9).Value = "Grand Total:";
        worksheet.Cell(row, 9).Style.Font.Bold = true;
        worksheet.Cell(row, 10).Value = grandTotalCharges;
        worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(row, 10).Style.Font.Bold = true;
        worksheet.Cell(row, 11).Value = grandTotalPaid;
        worksheet.Cell(row, 11).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(row, 11).Style.Font.Bold = true;
        worksheet.Cell(row, 12).Value = grandTotalCharges - grandTotalPaid;
        worksheet.Cell(row, 12).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(row, 12).Style.Font.Bold = true;
        worksheet.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#FEF3C7");

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();
        
        // Add table borders
        var dataRange = worksheet.Range(3, 1, row, headers.Length);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;
using System.Security.Claims;
using System.Globalization;

namespace MessManagement.Areas.User.Pages;

[Authorize(Roles = "User")]
public class MyBillModel : PageModel
{
    private readonly MessDbContext _context;

    public MyBillModel(MessDbContext context)
    {
        _context = context;
    }

    public string MemberName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public int SelectedMonth { get; set; }
    public int SelectedYear { get; set; }
    public string MonthName { get; set; } = "";

    // Attendance
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }

    // Consumption
    public int WaterCount { get; set; }
    public int TeaCount { get; set; }

    // Rates
    public decimal MealRate { get; set; } = 250;
    public decimal WaterRate { get; set; } = 50;
    public decimal TeaRate { get; set; } = 30;

    // Charges
    public decimal MealCharges { get; set; }
    public decimal WaterCharges { get; set; }
    public decimal TeaCharges { get; set; }
    public decimal GrandTotal { get; set; }

    // Payments
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public List<Payment> Payments { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? month, int? year)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return RedirectToPage("/Account/Login");

        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return RedirectToPage("/Account/Login");

        MemberName = member.FullName;
        RoomNumber = member.RoomNumber;

        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;
        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth);

        var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Get attendance
        var attendance = await _context.Attendances
            .Where(a => a.MemberId == member.MemberId && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        PresentDays = attendance.Count(a => a.Status == AttendanceStatus.Present);
        AbsentDays = attendance.Count(a => a.Status == AttendanceStatus.Absent);

        // Get water/tea consumption
        var waterTea = await _context.WaterTeaRecords
            .Where(w => w.MemberId == member.MemberId && w.Date >= startDate && w.Date <= endDate)
            .ToListAsync();

        WaterCount = waterTea.Sum(w => w.WaterCount);
        TeaCount = waterTea.Sum(w => w.TeaCount);

        // Calculate charges
        MealCharges = PresentDays * MealRate;
        WaterCharges = WaterCount * WaterRate;
        TeaCharges = TeaCount * TeaRate;
        GrandTotal = MealCharges + WaterCharges + TeaCharges;

        // Get payments for this month
        Payments = await _context.Payments
            .Where(p => p.MemberId == member.MemberId && p.Date >= startDate && p.Date <= endDate)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        AmountPaid = Payments.Sum(p => p.Amount);
        Balance = GrandTotal - AmountPaid;

        return Page();
    }
}

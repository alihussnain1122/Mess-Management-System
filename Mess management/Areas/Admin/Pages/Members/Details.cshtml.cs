using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;
using MessManagement.Helpers;
using System.Globalization;

namespace MessManagement.Areas.Admin.Pages.Members;

[Authorize(Roles = "Admin")]
public class DetailsModel : PageModel
{
    private readonly MessDbContext _context;

    public DetailsModel(MessDbContext context)
    {
        _context = context;
    }

    public Member? Member { get; set; }
    public int SelectedMonth { get; set; }
    public int SelectedYear { get; set; }
    public string MonthName { get; set; } = "";

    // Attendance Records
    public List<Models.Attendance> AttendanceRecords { get; set; } = new();
    
    // Meal-wise counts
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    public int TotalMeals { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }

    // Rates
    public decimal BreakfastRate { get; set; } = Constants.DefaultBreakfastRate;
    public decimal LunchRate { get; set; } = Constants.DefaultLunchRate;
    public decimal DinnerRate { get; set; } = Constants.DefaultDinnerRate;
    public decimal TeaRate { get; set; } = Constants.DefaultTeaCost;

    // Charges
    public decimal BreakfastCharges { get; set; }
    public decimal LunchCharges { get; set; }
    public decimal DinnerCharges { get; set; }
    public decimal MealCharges { get; set; }
    public decimal TeaCharges { get; set; }
    public decimal GrandTotal { get; set; }

    // Payments
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public List<Payment> Payments { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, int? month, int? year)
    {
        Member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.MemberId == id);

        if (Member == null)
            return NotFound();

        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;
        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth);

        var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Get attendance records
        AttendanceRecords = await _context.Attendances
            .Where(a => a.MemberId == id && a.Date >= startDate && a.Date <= endDate)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        // Calculate meal-wise counts
        BreakfastCount = AttendanceRecords.Count(a => a.BreakfastPresent);
        LunchCount = AttendanceRecords.Count(a => a.LunchPresent);
        DinnerCount = AttendanceRecords.Count(a => a.DinnerPresent);
        TotalMeals = BreakfastCount + LunchCount + DinnerCount;
        PresentDays = AttendanceRecords.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);
        AbsentDays = AttendanceRecords.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent);

        // Calculate charges
        BreakfastCharges = BreakfastCount * BreakfastRate;
        LunchCharges = LunchCount * LunchRate;
        DinnerCharges = DinnerCount * DinnerRate;
        MealCharges = BreakfastCharges + LunchCharges + DinnerCharges;
        TeaCharges = TotalMeals * TeaRate;
        GrandTotal = MealCharges + TeaCharges;

        // Get payments for this month
        Payments = await _context.Payments
            .Where(p => p.MemberId == id && p.Date >= startDate && p.Date <= endDate && p.Status == PaymentStatus.Completed)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        AmountPaid = Payments.Sum(p => p.Amount);
        Balance = GrandTotal - AmountPaid;

        return Page();
    }
}

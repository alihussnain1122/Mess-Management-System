using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Helpers;
using System.Globalization;

namespace MessManagement.Areas.Admin.Pages.Members;

[Authorize(Roles = "Admin")]
public class BillModel : PageModel
{
    private readonly MessDbContext _context;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;

    public BillModel(MessDbContext context, IPdfService pdfService, IEmailService emailService)
    {
        _context = context;
        _pdfService = pdfService;
        _emailService = emailService;
    }

    public Member? Member { get; set; }

    [BindProperty]
    public int SelectedMonth { get; set; } = DateTime.Now.Month;

    [BindProperty]
    public int SelectedYear { get; set; } = DateTime.Now.Year;

    [BindProperty]
    public decimal BreakfastRate { get; set; } = Constants.DefaultBreakfastRate;

    [BindProperty]
    public decimal LunchRate { get; set; } = Constants.DefaultLunchRate;

    [BindProperty]
    public decimal DinnerRate { get; set; } = Constants.DefaultDinnerRate;

    [BindProperty]
    public decimal WaterRate { get; set; } = Constants.DefaultWaterCost;

    [BindProperty]
    public decimal TeaRate { get; set; } = Constants.DefaultTeaCost;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Member = await _context.Members.FindAsync(id);
        
        if (Member == null)
        {
            return Page();
        }

        return Page();
    }

    private async Task<MealWiseBillData?> GetBillDataAsync(int id)
    {
        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.MemberId == id);
        
        if (member == null)
            return null;

        var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var attendance = await _context.Attendances
            .Where(a => a.MemberId == id && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        var breakfastCount = attendance.Count(a => a.BreakfastPresent);
        var lunchCount = attendance.Count(a => a.LunchPresent);
        var dinnerCount = attendance.Count(a => a.DinnerPresent);
        var presentDays = attendance.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);
        var absentDays = attendance.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent);

        var waterTea = await _context.WaterTeaRecords
            .Where(w => w.MemberId == id && w.Date >= startDate && w.Date <= endDate)
            .ToListAsync();

        var waterCount = waterTea.Sum(w => w.WaterCount);
        var teaCount = waterTea.Sum(w => w.TeaCount);

        var payments = await _context.Payments
            .Where(p => p.MemberId == id && p.Date >= startDate && p.Date <= endDate)
            .OrderBy(p => p.Date)
            .ToListAsync();

        var breakfastCharges = breakfastCount * BreakfastRate;
        var lunchCharges = lunchCount * LunchRate;
        var dinnerCharges = dinnerCount * DinnerRate;
        var mealCharges = breakfastCharges + lunchCharges + dinnerCharges;
        var waterCharges = waterCount * WaterRate;
        var teaCharges = teaCount * TeaRate;
        var grandTotal = mealCharges + waterCharges + teaCharges;
        var amountPaid = payments.Sum(p => p.Amount);
        var balance = grandTotal - amountPaid;

        return new MealWiseBillData
        {
            Member = member,
            BreakfastCount = breakfastCount,
            LunchCount = lunchCount,
            DinnerCount = dinnerCount,
            PresentDays = presentDays,
            AbsentDays = absentDays,
            WaterCount = waterCount,
            TeaCount = teaCount,
            BreakfastCharges = breakfastCharges,
            LunchCharges = lunchCharges,
            DinnerCharges = dinnerCharges,
            MealCharges = mealCharges,
            WaterCharges = waterCharges,
            TeaCharges = teaCharges,
            GrandTotal = grandTotal,
            AmountPaid = amountPaid,
            Balance = balance,
            Payments = payments
        };
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var data = await GetBillDataAsync(id);
        if (data == null)
            return NotFound();

        Member = data.Member;
        var totalDays = DateTime.DaysInMonth(SelectedYear, SelectedMonth);

        var bill = new MemberMonthlyBillViewModel
        {
            MemberName = data.Member.FullName,
            RoomNumber = data.Member.RoomNumber,
            Email = data.Member.User?.Email ?? "",
            Phone = "",
            Month = SelectedMonth,
            Year = SelectedYear,
            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth),
            TotalDays = totalDays,
            PresentDays = data.PresentDays,
            AbsentDays = data.AbsentDays,
            // Meal-wise data
            BreakfastCount = data.BreakfastCount,
            LunchCount = data.LunchCount,
            DinnerCount = data.DinnerCount,
            BreakfastRate = BreakfastRate,
            LunchRate = LunchRate,
            DinnerRate = DinnerRate,
            BreakfastCharges = data.BreakfastCharges,
            LunchCharges = data.LunchCharges,
            DinnerCharges = data.DinnerCharges,
            MealRate = BreakfastRate + LunchRate + DinnerRate, // Legacy
            TotalMealCharges = data.MealCharges,
            WaterCount = data.WaterCount,
            WaterRate = WaterRate,
            WaterCharges = data.WaterCharges,
            TeaCount = data.TeaCount,
            TeaRate = TeaRate,
            TeaCharges = data.TeaCharges,
            GrandTotal = data.GrandTotal,
            AmountPaid = data.AmountPaid,
            Balance = data.Balance,
            Payments = data.Payments.Select(p => new PaymentStatementItem
            {
                Date = p.Date,
                Description = p.Notes ?? $"Payment - {p.PaymentMode}",
                Amount = p.Amount,
                Status = "Paid"
            }).ToList()
        };

        var pdfBytes = _pdfService.GenerateMemberMonthlyBill(bill);

        return File(pdfBytes, "application/pdf", $"Bill_{data.Member.FullName.Replace(" ", "_")}_{SelectedMonth}_{SelectedYear}.pdf");
    }

    public async Task<IActionResult> OnPostSendEmailAsync(int id)
    {
        var data = await GetBillDataAsync(id);
        if (data == null)
        {
            TempData["ToastError"] = "Member not found!";
            return RedirectToPage(new { id });
        }

        Member = data.Member;

        var email = data.Member.User?.Email;
        if (string.IsNullOrEmpty(email))
        {
            TempData["ToastError"] = "Member does not have an email address!";
            return RedirectToPage(new { id });
        }

        var billModel = new MonthlyBillEmailModel
        {
            MemberName = data.Member.FullName,
            RoomNumber = data.Member.RoomNumber,
            Month = SelectedMonth,
            Year = SelectedYear,
            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth),
            PresentDays = data.PresentDays,
            AbsentDays = data.AbsentDays,
            // Meal-wise data
            BreakfastCount = data.BreakfastCount,
            LunchCount = data.LunchCount,
            DinnerCount = data.DinnerCount,
            BreakfastRate = BreakfastRate,
            LunchRate = LunchRate,
            DinnerRate = DinnerRate,
            BreakfastCharges = data.BreakfastCharges,
            LunchCharges = data.LunchCharges,
            DinnerCharges = data.DinnerCharges,
            MealRate = BreakfastRate + LunchRate + DinnerRate,
            MealCharges = data.MealCharges,
            WaterCount = data.WaterCount,
            WaterRate = WaterRate,
            WaterCharges = data.WaterCharges,
            TeaCount = data.TeaCount,
            TeaRate = TeaRate,
            TeaCharges = data.TeaCharges,
            GrandTotal = data.GrandTotal,
            AmountPaid = data.AmountPaid,
            Balance = data.Balance
        };

        await _emailService.SendMonthlyBillAsync(email, data.Member.FullName, billModel);

        TempData["ToastSuccess"] = $"Bill email sent successfully to {email}!";
        return RedirectToPage(new { id });
    }
}

public class MealWiseBillData
{
    public Member Member { get; set; } = null!;
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int WaterCount { get; set; }
    public int TeaCount { get; set; }
    public decimal BreakfastCharges { get; set; }
    public decimal LunchCharges { get; set; }
    public decimal DinnerCharges { get; set; }
    public decimal MealCharges { get; set; }
    public decimal WaterCharges { get; set; }
    public decimal TeaCharges { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public List<Payment> Payments { get; set; } = new();
}

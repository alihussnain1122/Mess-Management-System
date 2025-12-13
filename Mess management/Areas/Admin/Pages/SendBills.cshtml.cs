using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using System.Globalization;

namespace MessManagement.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class SendBillsModel : PageModel
{
    private readonly MessDbContext _context;
    private readonly IEmailService _emailService;

    public SendBillsModel(MessDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public List<Member> Members { get; set; } = new();

    [BindProperty]
    public int SelectedMonth { get; set; } = DateTime.Now.Month;

    [BindProperty]
    public int SelectedYear { get; set; } = DateTime.Now.Year;

    [BindProperty]
    public decimal MealRate { get; set; } = 250;

    [BindProperty]
    public decimal WaterRate { get; set; } = 50;

    [BindProperty]
    public decimal TeaRate { get; set; } = 30;

    [BindProperty]
    public List<int> SelectedMemberIds { get; set; } = new();

    public async Task OnGetAsync()
    {
        Members = await _context.Members
            .Where(m => m.IsActive)
            .Include(m => m.User)
            .OrderBy(m => m.FullName)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (SelectedMemberIds == null || !SelectedMemberIds.Any())
        {
            TempData["ToastError"] = "Please select at least one member!";
            return RedirectToPage();
        }

        var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth);

        var members = await _context.Members
            .Where(m => SelectedMemberIds.Contains(m.MemberId))
            .Include(m => m.User)
            .ToListAsync();

        var attendances = await _context.Attendances
            .Where(a => SelectedMemberIds.Contains(a.MemberId) && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        var waterTeas = await _context.WaterTeaRecords
            .Where(w => SelectedMemberIds.Contains(w.MemberId) && w.Date >= startDate && w.Date <= endDate)
            .ToListAsync();

        var payments = await _context.Payments
            .Where(p => SelectedMemberIds.Contains(p.MemberId) && p.Date >= startDate && p.Date <= endDate)
            .ToListAsync();

        int successCount = 0;
        int failCount = 0;

        foreach (var member in members)
        {
            var email = member.User?.Email;
            if (string.IsNullOrEmpty(email))
            {
                failCount++;
                continue;
            }

            var memberAttendance = attendances.Where(a => a.MemberId == member.MemberId).ToList();
            var presentDays = memberAttendance.Count(a => a.Status == AttendanceStatus.Present);
            var absentDays = memberAttendance.Count(a => a.Status == AttendanceStatus.Absent);

            var memberWaterTea = waterTeas.Where(w => w.MemberId == member.MemberId).ToList();
            var waterCount = memberWaterTea.Sum(w => w.WaterCount);
            var teaCount = memberWaterTea.Sum(w => w.TeaCount);

            var mealCharges = presentDays * MealRate;
            var waterCharges = waterCount * WaterRate;
            var teaCharges = teaCount * TeaRate;
            var grandTotal = mealCharges + waterCharges + teaCharges;

            var memberPayments = payments.Where(p => p.MemberId == member.MemberId).Sum(p => p.Amount);
            var balance = grandTotal - memberPayments;

            var billModel = new MonthlyBillEmailModel
            {
                MemberName = member.FullName,
                RoomNumber = member.RoomNumber,
                Month = SelectedMonth,
                Year = SelectedYear,
                MonthName = monthName,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                MealRate = MealRate,
                MealCharges = mealCharges,
                WaterCount = waterCount,
                WaterRate = WaterRate,
                WaterCharges = waterCharges,
                TeaCount = teaCount,
                TeaRate = TeaRate,
                TeaCharges = teaCharges,
                GrandTotal = grandTotal,
                AmountPaid = memberPayments,
                Balance = balance
            };

            try
            {
                await _emailService.SendMonthlyBillAsync(email, member.FullName, billModel);
                successCount++;
            }
            catch
            {
                failCount++;
            }
        }

        if (successCount > 0 && failCount == 0)
        {
            TempData["ToastSuccess"] = $"Bills sent successfully to {successCount} member(s)!";
        }
        else if (successCount > 0 && failCount > 0)
        {
            TempData["ToastInfo"] = $"Bills sent to {successCount} member(s). {failCount} failed.";
        }
        else
        {
            TempData["ToastError"] = "Failed to send bills. Please check email configuration.";
        }

        return RedirectToPage();
    }
}

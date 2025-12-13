using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Helpers;
using MessManagement.ViewModels;
using System.Globalization;
using System.Collections.Concurrent;

namespace MessManagement.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class SendBillsModel : PageModel
{
    private readonly MessDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IPdfService _pdfService;
    private readonly ILogger<SendBillsModel> _logger;

    public SendBillsModel(
        MessDbContext context, 
        IEmailService emailService, 
        IPdfService pdfService,
        ILogger<SendBillsModel> logger)
    {
        _context = context;
        _emailService = emailService;
        _pdfService = pdfService;
        _logger = logger;
    }

    public List<Member> Members { get; set; } = new();

    [BindProperty]
    public int SelectedMonth { get; set; } = DateTime.Now.Month;

    [BindProperty]
    public int SelectedYear { get; set; } = DateTime.Now.Year;

    // Water & Tea rates (auto-included)
    public decimal WaterRate { get; set; } = Constants.DefaultWaterCost;  // FREE
    public decimal TeaRate { get; set; } = Constants.DefaultTeaCost;       // Rs.100

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

    // Get menu price for a specific day and meal type
    private decimal GetMenuPrice(List<WeeklyMenu> menus, DayOfWeek dayOfWeek, MealType mealType)
    {
        var menu = menus.FirstOrDefault(m => m.DayOfWeek == dayOfWeek && m.MealType == mealType);
        return menu?.Price ?? 0;
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
        var totalDaysInMonth = DateTime.DaysInMonth(SelectedYear, SelectedMonth);

        var members = await _context.Members
            .Where(m => SelectedMemberIds.Contains(m.MemberId))
            .Include(m => m.User)
            .ToListAsync();

        var attendances = await _context.Attendances
            .Where(a => SelectedMemberIds.Contains(a.MemberId) && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        var payments = await _context.Payments
            .Where(p => SelectedMemberIds.Contains(p.MemberId) && p.Date >= startDate && p.Date <= endDate)
            .ToListAsync();

        // Load weekly menus for price lookup
        var weeklyMenus = await _context.WeeklyMenus.ToListAsync();

        // Use concurrent collections for thread safety
        var successCount = new ConcurrentBag<string>();
        var failedMembers = new ConcurrentBag<string>();

        // Prepare bill data for each member
        var billTasks = members.Select(async member =>
        {
            var email = member.User?.Email;
            if (string.IsNullOrEmpty(email))
            {
                failedMembers.Add($"{member.FullName} (No email)");
                return;
            }

            try
            {
                var memberAttendance = attendances.Where(a => a.MemberId == member.MemberId).ToList();
                
                // Meal-wise counts
                var breakfastCount = memberAttendance.Count(a => a.BreakfastPresent);
                var lunchCount = memberAttendance.Count(a => a.LunchPresent);
                var dinnerCount = memberAttendance.Count(a => a.DinnerPresent);
                var totalMeals = breakfastCount + lunchCount + dinnerCount;
                var presentDays = memberAttendance.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);
                var absentDays = totalDaysInMonth - presentDays;

                // Water & Tea are auto-included with every meal
                var waterCount = totalMeals;  // FREE
                var teaCount = totalMeals;    // Rs.100 per meal

                // Calculate charges based on menu prices for each day
                decimal breakfastCharges = 0;
                decimal lunchCharges = 0;
                decimal dinnerCharges = 0;

                foreach (var att in memberAttendance)
                {
                    var dayOfWeek = att.Date.DayOfWeek;
                    if (att.BreakfastPresent)
                        breakfastCharges += GetMenuPrice(weeklyMenus, dayOfWeek, MealType.Breakfast);
                    if (att.LunchPresent)
                        lunchCharges += GetMenuPrice(weeklyMenus, dayOfWeek, MealType.Lunch);
                    if (att.DinnerPresent)
                        dinnerCharges += GetMenuPrice(weeklyMenus, dayOfWeek, MealType.Dinner);
                }

                var mealCharges = breakfastCharges + lunchCharges + dinnerCharges;
                var waterCharges = waterCount * WaterRate;  // Rs.0
                var teaCharges = teaCount * TeaRate;        // Rs.100 per meal
                var grandTotal = mealCharges + waterCharges + teaCharges;

                // Calculate average rates for display
                var avgBreakfastRate = breakfastCount > 0 ? breakfastCharges / breakfastCount : 0;
                var avgLunchRate = lunchCount > 0 ? lunchCharges / lunchCount : 0;
                var avgDinnerRate = dinnerCount > 0 ? dinnerCharges / dinnerCount : 0;

                var memberPayments = payments.Where(p => p.MemberId == member.MemberId).ToList();
                var amountPaid = memberPayments.Sum(p => p.Amount);
                var balance = grandTotal - amountPaid;

                // Daily attendance for PDF
                var dailyAttendance = memberAttendance.Select(a => new DailyAttendanceItem
                {
                    Date = a.Date,
                    Breakfast = a.BreakfastPresent,
                    Lunch = a.LunchPresent,
                    Dinner = a.DinnerPresent,
                    Status = (a.BreakfastPresent || a.LunchPresent || a.DinnerPresent) ? "Present" : "Absent"
                }).OrderBy(d => d.Date).ToList();

                // Payment history for PDF
                var paymentHistory = memberPayments.Select(p => new PaymentStatementItem
                {
                    Date = p.Date,
                    Description = p.PaymentMode.ToString(),
                    Amount = p.Amount,
                    Status = "Paid"
                }).OrderBy(p => p.Date).ToList();

                // Generate PDF bill
                var pdfBillModel = new MemberMonthlyBillViewModel
                {
                    MemberName = member.FullName,
                    RoomNumber = member.RoomNumber,
                    Email = email,
                    Phone = "",
                    Month = SelectedMonth,
                    Year = SelectedYear,
                    MonthName = monthName,
                    TotalDays = totalDaysInMonth,
                    PresentDays = presentDays,
                    AbsentDays = absentDays,
                    BreakfastCount = breakfastCount,
                    LunchCount = lunchCount,
                    DinnerCount = dinnerCount,
                    BreakfastRate = avgBreakfastRate,
                    LunchRate = avgLunchRate,
                    DinnerRate = avgDinnerRate,
                    BreakfastCharges = breakfastCharges,
                    LunchCharges = lunchCharges,
                    DinnerCharges = dinnerCharges,
                    MealRate = avgBreakfastRate + avgLunchRate + avgDinnerRate,
                    TotalMealCharges = mealCharges,
                    WaterCount = waterCount,
                    WaterRate = WaterRate,
                    WaterCharges = waterCharges,
                    TeaCount = teaCount,
                    TeaRate = TeaRate,
                    TeaCharges = teaCharges,
                    GrandTotal = grandTotal,
                    AmountPaid = amountPaid,
                    Balance = balance,
                    DailyAttendance = dailyAttendance,
                    Payments = paymentHistory
                };

                var pdfBytes = _pdfService.GenerateMemberMonthlyBill(pdfBillModel);
                var fileName = $"Bill_{member.FullName.Replace(" ", "_")}_{monthName}_{SelectedYear}.pdf";

                // Email model
                var emailBillModel = new MonthlyBillEmailModel
                {
                    MemberName = member.FullName,
                    RoomNumber = member.RoomNumber,
                    Month = SelectedMonth,
                    Year = SelectedYear,
                    MonthName = monthName,
                    PresentDays = presentDays,
                    AbsentDays = absentDays,
                    BreakfastCount = breakfastCount,
                    LunchCount = lunchCount,
                    DinnerCount = dinnerCount,
                    BreakfastRate = avgBreakfastRate,
                    LunchRate = avgLunchRate,
                    DinnerRate = avgDinnerRate,
                    BreakfastCharges = breakfastCharges,
                    LunchCharges = lunchCharges,
                    DinnerCharges = dinnerCharges,
                    MealRate = avgBreakfastRate + avgLunchRate + avgDinnerRate,
                    MealCharges = mealCharges,
                    WaterCount = waterCount,
                    WaterRate = WaterRate,
                    WaterCharges = waterCharges,
                    TeaCount = teaCount,
                    TeaRate = TeaRate,
                    TeaCharges = teaCharges,
                    GrandTotal = grandTotal,
                    AmountPaid = amountPaid,
                    Balance = balance
                };

                // Send email with PDF attachment
                var success = await _emailService.SendMonthlyBillWithPdfAsync(email, member.FullName, emailBillModel, pdfBytes, fileName);
                
                if (success)
                {
                    successCount.Add(member.FullName);
                    _logger.LogInformation("Bill sent successfully to {MemberName} ({Email})", member.FullName, email);
                }
                else
                {
                    failedMembers.Add($"{member.FullName} (Email failed)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send bill to {MemberName}", member.FullName);
                failedMembers.Add($"{member.FullName} (Error)");
            }
        });

        // Execute all email tasks concurrently
        await Task.WhenAll(billTasks);

        var successTotal = successCount.Count;
        var failTotal = failedMembers.Count;

        if (successTotal > 0 && failTotal == 0)
        {
            TempData["ToastSuccess"] = $"✅ Bills with PDF sent successfully to {successTotal} member(s)!";
        }
        else if (successTotal > 0 && failTotal > 0)
        {
            TempData["ToastInfo"] = $"📧 Bills sent to {successTotal} member(s). {failTotal} failed: {string.Join(", ", failedMembers.Take(3))}";
        }
        else
        {
            TempData["ToastError"] = $"❌ Failed to send bills. {string.Join(", ", failedMembers.Take(5))}";
        }

        return RedirectToPage();
    }
}

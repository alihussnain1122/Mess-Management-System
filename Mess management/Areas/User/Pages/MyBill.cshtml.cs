using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;
using MessManagement.Models;
using MessManagement.Helpers;
using MessManagement.ViewModels;
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

    // Attendance - Meal-wise
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    public int TotalMeals { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }

    // Auto-included Water & Tea
    public int WaterCount { get; set; }  // = TotalMeals
    public int TeaCount { get; set; }    // = TotalMeals

    // Rates
    public decimal BreakfastRate { get; set; } = Constants.DefaultBreakfastRate;
    public decimal LunchRate { get; set; } = Constants.DefaultLunchRate;
    public decimal DinnerRate { get; set; } = Constants.DefaultDinnerRate;
    public decimal WaterRate { get; set; } = Constants.DefaultWaterCost;  // FREE
    public decimal TeaRate { get; set; } = Constants.DefaultTeaCost;       // Rs.100

    // Charges
    public decimal BreakfastCharges { get; set; }
    public decimal LunchCharges { get; set; }
    public decimal DinnerCharges { get; set; }
    public decimal MealCharges { get; set; }
    public decimal WaterCharges { get; set; }
    public decimal TeaCharges { get; set; }
    public decimal GrandTotal { get; set; }

    // Payments
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public List<Payment> Payments { get; set; } = new();

    // Detailed Bill with Dishes
    public List<DailyMealDetail> DailyMeals { get; set; } = new();

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

        // Get all menus (template and specific dates)
        var specificMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate >= startDate && m.MenuDate <= endDate)
            .ToListAsync();
        var templateMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate == null)
            .ToListAsync();

        // Meal-wise counts
        BreakfastCount = attendance.Count(a => a.BreakfastPresent);
        LunchCount = attendance.Count(a => a.LunchPresent);
        DinnerCount = attendance.Count(a => a.DinnerPresent);
        TotalMeals = BreakfastCount + LunchCount + DinnerCount;
        PresentDays = attendance.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);
        AbsentDays = attendance.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent);

        // Water & Tea are auto-included with every meal
        WaterCount = TotalMeals;  // FREE with every meal
        TeaCount = TotalMeals;    // Rs.100 per meal

        // Calculate charges
        BreakfastCharges = BreakfastCount * BreakfastRate;
        LunchCharges = LunchCount * LunchRate;
        DinnerCharges = DinnerCount * DinnerRate;
        MealCharges = BreakfastCharges + LunchCharges + DinnerCharges;
        WaterCharges = WaterCount * WaterRate;  // Rs.0 (FREE)
        TeaCharges = TeaCount * TeaRate;        // Rs.100 per meal
        GrandTotal = MealCharges + WaterCharges + TeaCharges;

        // Build detailed daily meal list with dishes
        foreach (var att in attendance.OrderBy(a => a.Date))
        {
            var dayDetail = new DailyMealDetail
            {
                Date = att.Date,
                DayName = att.Date.DayOfWeek.ToString(),
                Meals = new List<MealDetail>()
            };

            // Get menu for this date
            var menuForDate = specificMenus.Where(m => m.MenuDate!.Value.Date == att.Date.Date).ToList();
            if (!menuForDate.Any())
            {
                menuForDate = templateMenus.Where(m => m.DayOfWeek == att.Date.DayOfWeek).ToList();
            }

            // Breakfast
            if (att.BreakfastPresent)
            {
                var breakfastDish = menuForDate.FirstOrDefault(m => m.MealType == MealType.Breakfast);
                dayDetail.Meals.Add(new MealDetail
                {
                    MealType = MealType.Breakfast,
                    DishName = breakfastDish?.DishName ?? "Breakfast",
                    Price = breakfastDish?.Price ?? BreakfastRate,
                    WasPresent = true
                });
            }

            // Lunch
            if (att.LunchPresent)
            {
                var lunchDish = menuForDate.FirstOrDefault(m => m.MealType == MealType.Lunch);
                dayDetail.Meals.Add(new MealDetail
                {
                    MealType = MealType.Lunch,
                    DishName = lunchDish?.DishName ?? "Lunch",
                    Price = lunchDish?.Price ?? LunchRate,
                    WasPresent = true
                });
            }

            // Dinner
            if (att.DinnerPresent)
            {
                var dinnerDish = menuForDate.FirstOrDefault(m => m.MealType == MealType.Dinner);
                dayDetail.Meals.Add(new MealDetail
                {
                    MealType = MealType.Dinner,
                    DishName = dinnerDish?.DishName ?? "Dinner",
                    Price = dinnerDish?.Price ?? DinnerRate,
                    WasPresent = true
                });
            }

            dayDetail.DayTotal = dayDetail.Meals.Sum(m => m.Price);
            
            if (dayDetail.Meals.Any())
            {
                DailyMeals.Add(dayDetail);
            }
        }

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

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
public class BillDetailsModel : PageModel
{
    private readonly MessDbContext _context;

    public BillDetailsModel(MessDbContext context)
    {
        _context = context;
    }

    // Member Info
    public string MemberName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public int MemberId { get; set; }

    // Filter Parameters
    public int SelectedMonth { get; set; }
    public int SelectedYear { get; set; }
    public string MonthName { get; set; } = "";
    public DateTime? SelectedDate { get; set; }
    public string? MealFilter { get; set; }  // "all", "breakfast", "lunch", "dinner"

    // Summary
    public int TotalDays { get; set; }
    public int TotalMeals { get; set; }
    public decimal TotalCharges { get; set; }

    // Detailed Records
    public List<DailyBillRecord> DailyRecords { get; set; } = new();

    // Single Day View Data
    public SingleDayBill? SingleDayData { get; set; }

    // Rates
    public decimal BreakfastRate { get; set; } = Constants.DefaultBreakfastRate;
    public decimal LunchRate { get; set; } = Constants.DefaultLunchRate;
    public decimal DinnerRate { get; set; } = Constants.DefaultDinnerRate;
    public decimal TeaRate { get; set; } = Constants.DefaultTeaCost;

    public async Task<IActionResult> OnGetAsync(int? month, int? year, string? date, string? meal)
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
        MemberId = member.MemberId;

        SelectedMonth = month ?? DateTime.Now.Month;
        SelectedYear = year ?? DateTime.Now.Year;
        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth);
        MealFilter = meal ?? "all";

        // Parse date if provided
        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
        {
            SelectedDate = parsedDate;
        }

        var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Get attendance records
        var attendanceQuery = _context.Attendances
            .Where(a => a.MemberId == member.MemberId && a.Date >= startDate && a.Date <= endDate);

        // If specific date is selected
        if (SelectedDate.HasValue)
        {
            attendanceQuery = _context.Attendances
                .Where(a => a.MemberId == member.MemberId && a.Date.Date == SelectedDate.Value.Date);
        }

        var attendance = await attendanceQuery.OrderBy(a => a.Date).ToListAsync();

        // Get menus
        var specificMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate >= startDate && m.MenuDate <= endDate)
            .ToListAsync();
        var templateMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate == null)
            .ToListAsync();

        // Build daily records
        foreach (var att in attendance)
        {
            var menuForDate = specificMenus.Where(m => m.MenuDate!.Value.Date == att.Date.Date).ToList();
            if (!menuForDate.Any())
            {
                menuForDate = templateMenus.Where(m => m.DayOfWeek == att.Date.DayOfWeek).ToList();
            }

            var record = new DailyBillRecord
            {
                Date = att.Date,
                DayName = att.Date.DayOfWeek.ToString(),
                Meals = new List<MealBillItem>()
            };

            // Breakfast
            if (att.BreakfastPresent && (MealFilter == "all" || MealFilter == "breakfast"))
            {
                var dish = menuForDate.FirstOrDefault(m => m.MealType == MealType.Breakfast);
                record.Meals.Add(new MealBillItem
                {
                    MealType = "Breakfast",
                    MealIcon = "🍳",
                    DishName = dish?.DishName ?? "Breakfast",
                    Description = "",
                    MealPrice = dish?.Price ?? BreakfastRate,
                    TeaPrice = TeaRate,
                    TotalPrice = (dish?.Price ?? BreakfastRate) + TeaRate
                });
            }

            // Lunch
            if (att.LunchPresent && (MealFilter == "all" || MealFilter == "lunch"))
            {
                var dish = menuForDate.FirstOrDefault(m => m.MealType == MealType.Lunch);
                record.Meals.Add(new MealBillItem
                {
                    MealType = "Lunch",
                    MealIcon = "🍛",
                    DishName = dish?.DishName ?? "Lunch",
                    Description = "",
                    MealPrice = dish?.Price ?? LunchRate,
                    TeaPrice = TeaRate,
                    TotalPrice = (dish?.Price ?? LunchRate) + TeaRate
                });
            }

            // Dinner
            if (att.DinnerPresent && (MealFilter == "all" || MealFilter == "dinner"))
            {
                var dish = menuForDate.FirstOrDefault(m => m.MealType == MealType.Dinner);
                record.Meals.Add(new MealBillItem
                {
                    MealType = "Dinner",
                    MealIcon = "🍽️",
                    DishName = dish?.DishName ?? "Dinner",
                    Description = "",
                    MealPrice = dish?.Price ?? DinnerRate,
                    TeaPrice = TeaRate,
                    TotalPrice = (dish?.Price ?? DinnerRate) + TeaRate
                });
            }

            record.DayMealTotal = record.Meals.Sum(m => m.MealPrice);
            record.DayTeaTotal = record.Meals.Sum(m => m.TeaPrice);
            record.DayGrandTotal = record.Meals.Sum(m => m.TotalPrice);
            record.MealCount = record.Meals.Count;

            if (record.Meals.Any())
            {
                DailyRecords.Add(record);
            }
        }

        // Calculate totals
        TotalDays = DailyRecords.Count;
        TotalMeals = DailyRecords.Sum(d => d.MealCount);
        TotalCharges = DailyRecords.Sum(d => d.DayGrandTotal);

        // If single date selected, prepare detailed single day view
        if (SelectedDate.HasValue && DailyRecords.Any())
        {
            var dayRecord = DailyRecords.First();
            SingleDayData = new SingleDayBill
            {
                Date = dayRecord.Date,
                DayName = dayRecord.DayName,
                Meals = dayRecord.Meals,
                MealTotal = dayRecord.DayMealTotal,
                TeaTotal = dayRecord.DayTeaTotal,
                WaterTotal = 0,  // FREE
                GrandTotal = dayRecord.DayGrandTotal
            };
        }

        return Page();
    }
}

// View Models for Bill Details
public class DailyBillRecord
{
    public DateTime Date { get; set; }
    public string DayName { get; set; } = "";
    public List<MealBillItem> Meals { get; set; } = new();
    public int MealCount { get; set; }
    public decimal DayMealTotal { get; set; }
    public decimal DayTeaTotal { get; set; }
    public decimal DayGrandTotal { get; set; }
}

public class MealBillItem
{
    public string MealType { get; set; } = "";
    public string MealIcon { get; set; } = "";
    public string DishName { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal MealPrice { get; set; }
    public decimal TeaPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class SingleDayBill
{
    public DateTime Date { get; set; }
    public string DayName { get; set; } = "";
    public List<MealBillItem> Meals { get; set; } = new();
    public decimal MealTotal { get; set; }
    public decimal TeaTotal { get; set; }
    public decimal WaterTotal { get; set; }
    public decimal GrandTotal { get; set; }
}

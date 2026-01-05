using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;

namespace MessManagement.Areas.User.Pages;

[Authorize(Roles = "User")]
public class DashboardModel : PageModel
{
    private readonly MessDbContext _context;

    public DashboardModel(MessDbContext context)
    {
        _context = context;
    }

    public string MemberName { get; set; } = "";
    public decimal MonthCost { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public int PresentDays { get; set; }
    public IEnumerable<WeeklyMenu> TodayMenu { get; set; } = new List<WeeklyMenu>();
    public IEnumerable<Payment> RecentPayments { get; set; } = new List<Payment>();

    public async Task OnGetAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return;

        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null) return;

        MemberName = member.FullName;
        var memberId = member.MemberId;

        var now = DateTime.Now;
        var startDate = new DateTime(now.Year, now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Get attendance records for the month
        var attendance = await _context.Attendances
            .Where(a => a.MemberId == memberId && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        PresentDays = attendance.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);

        // Get menus for calculating actual costs
        var specificMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate >= startDate && m.MenuDate <= endDate)
            .ToListAsync();
        var templateMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate == null)
            .ToListAsync();

        // Get today's menu
        var today = DateTime.Today;
        TodayMenu = specificMenus.Where(m => m.MenuDate?.Date == today).ToList();
        if (!TodayMenu.Any())
        {
            TodayMenu = templateMenus.Where(m => m.DayOfWeek == today.DayOfWeek).ToList();
        }

        // Calculate actual meal costs based on menu prices
        decimal mealCharges = 0;
        int totalMeals = 0;

        foreach (var att in attendance)
        {
            var menuForDate = specificMenus.Where(m => m.MenuDate!.Value.Date == att.Date.Date).ToList();
            if (!menuForDate.Any())
            {
                menuForDate = templateMenus.Where(m => m.DayOfWeek == att.Date.DayOfWeek).ToList();
            }

            if (att.BreakfastPresent)
            {
                var breakfast = menuForDate.FirstOrDefault(m => m.MealType == MealType.Breakfast);
                mealCharges += breakfast?.Price ?? Helpers.Constants.DefaultBreakfastRate;
                totalMeals++;
            }

            if (att.LunchPresent)
            {
                var lunch = menuForDate.FirstOrDefault(m => m.MealType == MealType.Lunch);
                mealCharges += lunch?.Price ?? Helpers.Constants.DefaultLunchRate;
                totalMeals++;
            }

            if (att.DinnerPresent)
            {
                var dinner = menuForDate.FirstOrDefault(m => m.MealType == MealType.Dinner);
                mealCharges += dinner?.Price ?? Helpers.Constants.DefaultDinnerRate;
                totalMeals++;
            }
        }

        // Tea is auto-included with every meal (Rs.100 per meal)
        var teaCharges = totalMeals * Helpers.Constants.DefaultTeaCost;

        // Get all payments for this member
        var allPayments = await _context.Payments
            .Where(p => p.MemberId == memberId)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        // Calculate total paid (only completed payments for current month)
        TotalPaid = allPayments
            .Where(p => p.Date.Month == now.Month && p.Date.Year == now.Year && p.Status == PaymentStatus.Completed)
            .Sum(p => p.Amount);
            
        MonthCost = mealCharges + teaCharges;
        BalanceDue = Math.Max(0, MonthCost - TotalPaid);
        
        // Recent payments (show all recent including pending)
        RecentPayments = allPayments.Take(5);
    }
}
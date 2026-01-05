using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MessManagement.Data;

namespace MessManagement.Areas.User.Pages.Payments;

[Authorize(Roles = "User")]
public class PayNowModel : PageModel
{
    private readonly MessDbContext _context;

    public PayNowModel(MessDbContext context)
    {
        _context = context;
    }

    public decimal MonthCost { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostAsync(decimal Amount, string PaymentMethod)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Page();

        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null) return Page();

        var payment = new Payment
        {
            MemberId = member.MemberId,
            Amount = Amount,
            PaymentMode = PaymentMethod == "Cash" ? PaymentMode.Cash : PaymentMode.Online,
            Date = DateTime.Now,
            Status = PaymentStatus.Pending // Payment requires admin verification
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Payment submitted successfully! It will be verified by admin shortly.";
        return RedirectToPage("/Dashboard", new { area = "User" });
    }

    private async Task LoadDataAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return;

        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null) return;

        var now = DateTime.Now;
        var startDate = new DateTime(now.Year, now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        // Get attendance records
        var attendance = await _context.Attendances
            .Where(a => a.MemberId == member.MemberId && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        // Get menus for the month
        var specificMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate != null && m.MenuDate >= startDate && m.MenuDate <= endDate)
            .ToListAsync();
        var templateMenus = await _context.WeeklyMenus
            .Where(m => m.MenuDate == null)
            .ToListAsync();

        // Calculate actual meal costs based on menu prices
        decimal mealCharges = 0;
        int totalMeals = 0;

        foreach (var att in attendance)
        {
            // Get menu for this date
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
        
        // Get completed payments for this month
        var payments = await _context.Payments
            .Where(p => p.MemberId == member.MemberId && 
                        p.Date.Month == now.Month && 
                        p.Date.Year == now.Year && 
                        p.Status == PaymentStatus.Completed)
            .ToListAsync();
            
        TotalPaid = payments.Sum(p => p.Amount);
        MonthCost = mealCharges + teaCharges;
        BalanceDue = Math.Max(0, MonthCost - TotalPaid);
    }
}
using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.ViewModels;
using MessManagement.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Services;

public class ReportService : IReportService
{
    private readonly MessDbContext _context;
    private readonly IMemberService _memberService;
    private readonly IAttendanceService _attendanceService;
    private readonly IPaymentService _paymentService;
    private readonly IMenuService _menuService;
    private readonly IWaterTeaService _waterTeaService;

    public ReportService(
        MessDbContext context,
        IMemberService memberService,
        IAttendanceService attendanceService,
        IPaymentService paymentService,
        IMenuService menuService,
        IWaterTeaService waterTeaService)
    {
        _context = context;
        _memberService = memberService;
        _attendanceService = attendanceService;
        _paymentService = paymentService;
        _menuService = menuService;
        _waterTeaService = waterTeaService;
    }

    public async Task<DailyReport> GetDailyReportAsync(DateTime date)
    {
        var attendances = await _attendanceService.GetAttendanceForDateAsync(date);
        var payments = await _paymentService.GetPaymentsByDateRangeAsync(date.Date, date.Date.AddDays(1).AddSeconds(-1));
        var menuItems = await _menuService.GetMenuByDayAsync(date.DayOfWeek);

        var attendanceList = attendances.ToList();
        var presentCount = attendanceList.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);
        var absentCount = attendanceList.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent);

        return new DailyReport
        {
            Date = date,
            TotalMembers = await _memberService.GetActiveMembersCountAsync(),
            PresentCount = presentCount,
            AbsentCount = absentCount,
            BreakfastCount = attendanceList.Count(a => a.BreakfastPresent),
            LunchCount = attendanceList.Count(a => a.LunchPresent),
            DinnerCount = attendanceList.Count(a => a.DinnerPresent),
            TotalPaymentsReceived = payments.Sum(p => p.Amount),
            PaymentCount = payments.Count(),
            MenuItems = menuItems.Select(m => new MenuItemViewModel
            {
                DishName = m.DishName,
                Price = m.Price,
                MealType = m.MealType.ToString()
            }).ToList()
        };
    }

    public async Task<MonthlyReport> GetMonthlyReportAsync(int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var totalRevenue = await _paymentService.GetTotalRevenueAsync(month, year);
        var activeMembers = await _memberService.GetActiveMembersCountAsync();

        var attendances = await _context.Attendances
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        var totalPresentDays = attendances.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);
        var totalAbsentDays = attendances.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent);
        
        var totalBreakfasts = attendances.Count(a => a.BreakfastPresent);
        var totalLunches = attendances.Count(a => a.LunchPresent);
        var totalDinners = attendances.Count(a => a.DinnerPresent);

        // Calculate expected revenue based on meal rates
        var expectedRevenue = (totalBreakfasts * Constants.DefaultBreakfastRate) +
                             (totalLunches * Constants.DefaultLunchRate) +
                             (totalDinners * Constants.DefaultDinnerRate);

        return new MonthlyReport
        {
            Month = month,
            Year = year,
            TotalActiveMembers = activeMembers,
            TotalPresentDays = totalPresentDays,
            TotalAbsentDays = totalAbsentDays,
            TotalBreakfasts = totalBreakfasts,
            TotalLunches = totalLunches,
            TotalDinners = totalDinners,
            TotalRevenue = totalRevenue,
            AverageDailyCost = Constants.DefaultBreakfastRate + Constants.DefaultLunchRate + Constants.DefaultDinnerRate,
            ExpectedRevenue = expectedRevenue
        };
    }

    public async Task<MemberCostBreakdown> GetMemberCostBreakdownAsync(int memberId, int month, int year)
    {
        var member = await _memberService.GetMemberByIdAsync(memberId);
        var mealSummary = await _attendanceService.GetMealWiseAttendanceSummaryAsync(memberId, month, year);
        var paymentSummary = await _paymentService.GetPaymentSummaryAsync(memberId, month, year);

        // Calculate meal costs based on individual meal rates
        var breakfastCost = mealSummary.BreakfastCount * Constants.DefaultBreakfastRate;
        var lunchCost = mealSummary.LunchCount * Constants.DefaultLunchRate;
        var dinnerCost = mealSummary.DinnerCount * Constants.DefaultDinnerRate;
        var totalMealCost = breakfastCost + lunchCost + dinnerCost;

        // Water & Tea are auto-included with every meal
        var totalMeals = mealSummary.BreakfastCount + mealSummary.LunchCount + mealSummary.DinnerCount;
        var waterTeaCost = totalMeals * Constants.DefaultTeaCost;  // Water is FREE, Tea is Rs.100 per meal

        var presentDays = await _attendanceService.GetPresentCountForMemberAsync(memberId, month, year);
        var absentDays = await _attendanceService.GetAbsentCountForMemberAsync(memberId, month, year);

        return new MemberCostBreakdown
        {
            MemberId = memberId,
            MemberName = member?.FullName ?? "Unknown",
            RoomNumber = member?.RoomNumber ?? "N/A",
            Month = month,
            Year = year,
            TotalPresentDays = presentDays,
            TotalAbsentDays = absentDays,
            BreakfastCount = mealSummary.BreakfastCount,
            LunchCount = mealSummary.LunchCount,
            DinnerCount = mealSummary.DinnerCount,
            BreakfastCost = breakfastCost,
            LunchCost = lunchCost,
            DinnerCost = dinnerCost,
            MealCost = totalMealCost,
            WaterTeaCost = waterTeaCost,
            TotalCost = totalMealCost + waterTeaCost,
            TotalPaid = paymentSummary.TotalPaidAmount,
            BalanceDue = (totalMealCost + waterTeaCost) - paymentSummary.TotalPaidAmount
        };
    }

    public async Task<IEnumerable<MemberCostBreakdown>> GetAllMembersCostBreakdownAsync(int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Run queries sequentially to avoid DbContext concurrency issues
        var activeMembers = await _memberService.GetActiveMembersAsync();
        var allAttendances = await _context.Attendances
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();
        var allPayments = await _context.Payments
            .Where(p => p.Date >= startDate && p.Date <= endDate && p.Status == Models.PaymentStatus.Completed)
            .ToListAsync();

        var breakdowns = activeMembers.Select(member =>
        {
            var memberAttendances = allAttendances.Where(a => a.MemberId == member.MemberId).ToList();
            
            // Calculate meal counts
            var breakfastCount = memberAttendances.Count(a => a.BreakfastPresent);
            var lunchCount = memberAttendances.Count(a => a.LunchPresent);
            var dinnerCount = memberAttendances.Count(a => a.DinnerPresent);
            var totalMeals = breakfastCount + lunchCount + dinnerCount;
            
            // Calculate meal costs
            var breakfastCost = breakfastCount * Constants.DefaultBreakfastRate;
            var lunchCost = lunchCount * Constants.DefaultLunchRate;
            var dinnerCost = dinnerCount * Constants.DefaultDinnerRate;
            var mealCost = breakfastCost + lunchCost + dinnerCost;
            
            // Water & Tea are auto-included with every meal
            var waterTeaCost = totalMeals * Constants.DefaultTeaCost;  // Water FREE, Tea Rs.100 per meal
            
            var presentDays = memberAttendances.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);
            var absentDays = memberAttendances.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent);
            var totalPaid = allPayments.Where(p => p.MemberId == member.MemberId).Sum(p => p.Amount);

            return new MemberCostBreakdown
            {
                MemberId = member.MemberId,
                MemberName = member.FullName,
                RoomNumber = member.RoomNumber,
                Month = month,
                Year = year,
                TotalPresentDays = presentDays,
                TotalAbsentDays = absentDays,
                BreakfastCount = breakfastCount,
                LunchCount = lunchCount,
                DinnerCount = dinnerCount,
                BreakfastCost = breakfastCost,
                LunchCost = lunchCost,
                DinnerCost = dinnerCost,
                MealCost = mealCost,
                WaterTeaCost = waterTeaCost,
                TotalCost = mealCost + waterTeaCost,
                TotalPaid = totalPaid,
                BalanceDue = (mealCost + waterTeaCost) - totalPaid
            };
        }).OrderBy(b => b.MemberName).ToList();

        return breakdowns;
    }

    public async Task<WeeklyMenuSummary> GetWeeklyMenuSummaryAsync()
    {
        var menuItems = await _menuService.GetWeeklyMenuAsync();
        var costSummary = await _menuService.GetWeeklyMenuCostSummaryAsync();

        var dailySummaries = menuItems
            .GroupBy(m => m.DayOfWeek)
            .Select(g => new DailyMenuSummary
            {
                DayOfWeek = g.Key,
                Items = g.Select(m => new MenuItemViewModel
                {
                    DishName = m.DishName,
                    Price = m.Price,
                    MealType = m.MealType.ToString()
                }).ToList(),
                TotalCost = costSummary.GetValueOrDefault(g.Key, 0)
            })
            .OrderBy(d => d.DayOfWeek)
            .ToList();

        return new WeeklyMenuSummary
        {
            DailySummaries = dailySummaries,
            TotalWeeklyCost = costSummary.Values.Sum(),
            AverageDailyCost = costSummary.Values.Any() ? costSummary.Values.Average() : 0
        };
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync()
    {
        var today = DateTime.UtcNow.Date;
        var currentMonth = today.Month;
        var currentYear = today.Year;

        var todayAttendance = await _attendanceService.GetAttendanceForDateAsync(today);
        var monthlyReport = await GetMonthlyReportAsync(currentMonth, currentYear);
        var todayList = todayAttendance.ToList();

        // Get last 7 days attendance and revenue
        var weekStart = today.AddDays(-6);
        var weeklyAttendance = new List<DailyAttendanceStat>();
        var weeklyRevenue = new List<DailyRevenueStat>();

        for (var date = weekStart; date <= today; date = date.AddDays(1))
        {
            var dayAttendance = await _attendanceService.GetAttendanceForDateAsync(date);
            var dayPayments = await _paymentService.GetPaymentsByDateRangeAsync(date, date.AddDays(1).AddSeconds(-1));
            var dayList = dayAttendance.ToList();
            
            weeklyAttendance.Add(new DailyAttendanceStat
            {
                Day = date.ToString("ddd"),
                Present = dayList.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent),
                Absent = dayList.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent)
            });
            
            weeklyRevenue.Add(new DailyRevenueStat
            {
                Day = date.ToString("ddd"),
                Amount = dayPayments.Sum(p => p.Amount)
            });
        }

        return new DashboardSummary
        {
            TotalMembers = await _memberService.GetTotalMembersCountAsync(),
            ActiveMembers = await _memberService.GetActiveMembersCountAsync(),
            TodayPresentCount = todayList.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent),
            TodayAbsentCount = todayList.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent),
            MonthlyRevenue = monthlyReport.TotalRevenue,
            MonthlyExpectedRevenue = monthlyReport.ExpectedRevenue,
            CurrentMonth = currentMonth,
            CurrentYear = currentYear,
            WeeklyAttendance = weeklyAttendance,
            WeeklyRevenue = weeklyRevenue
        };
    }

    public async Task<IEnumerable<PaymentSummary>> GetPaymentReportAsync(DateTime startDate, DateTime endDate)
    {
        var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
        
        return payments
            .GroupBy(p => p.MemberId)
            .Select(g => new PaymentSummary
            {
                MemberId = g.Key,
                MemberName = g.First().Member?.FullName ?? "Unknown",
                TotalPaidAmount = g.Sum(p => p.Amount),
                PaymentCount = g.Count(),
                Payments = g.ToList()
            })
            .OrderBy(p => p.MemberName);
    }
}

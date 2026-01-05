namespace MessManagement.Helpers;

public static class ReportCalculator
{
    public static decimal CalculateAttendancePercentage(int presentDays, int totalDays)
    {
        if (totalDays <= 0) return 0;
        return Math.Round((decimal)presentDays / totalDays * 100, 2);
    }

    public static decimal CalculateMealCost(int presentDays, decimal dailyRate)
    {
        return Math.Round(presentDays * dailyRate, 2);
    }

    public static decimal CalculateWaterTeaCost(int waterCount, int teaCount, decimal waterRate = 5m, decimal teaRate = 10m)
    {
        return Math.Round((waterCount * waterRate) + (teaCount * teaRate), 2);
    }

    public static decimal CalculateTotalCost(decimal mealCost, decimal waterTeaCost, decimal additionalCharges = 0)
    {
        return Math.Round(mealCost + waterTeaCost + additionalCharges, 2);
    }

    public static decimal CalculateBalance(decimal totalCost, decimal totalPaid)
    {
        return Math.Round(totalCost - totalPaid, 2);
    }

    public static decimal CalculateCollectionEfficiency(decimal collected, decimal expected)
    {
        if (expected <= 0) return 0;
        return Math.Round((collected / expected) * 100, 2);
    }

    public static decimal CalculateAverageDailyCost(decimal totalCost, int days)
    {
        if (days <= 0) return 0;
        return Math.Round(totalCost / days, 2);
    }

    public static decimal CalculatePerMemberRevenue(decimal totalRevenue, int memberCount)
    {
        if (memberCount <= 0) return 0;
        return Math.Round(totalRevenue / memberCount, 2);
    }

    public static (decimal Min, decimal Max, decimal Average) CalculateStatistics(IEnumerable<decimal> values)
    {
        var list = values.ToList();
        if (!list.Any()) return (0, 0, 0);

        return (list.Min(), list.Max(), Math.Round(list.Average(), 2));
    }

    public static Dictionary<string, decimal> CalculatePaymentModeBreakdown(IEnumerable<(string Mode, decimal Amount)> payments)
    {
        return payments
            .GroupBy(p => p.Mode)
            .ToDictionary(g => g.Key, g => Math.Round(g.Sum(p => p.Amount), 2));
    }

    /// <summary>
    /// Calculate meal cost from actual attendance and menu prices
    /// </summary>
    public static decimal CalculateMealCostFromMenu(
        List<(DateTime Date, bool BreakfastPresent, bool LunchPresent, bool DinnerPresent)> attendance,
        List<(DateTime? MenuDate, DayOfWeek DayOfWeek, Models.MealType MealType, decimal Price)> menus)
    {
        decimal totalCost = 0;

        foreach (var att in attendance)
        {
            // Find menu for this date (specific date menu takes precedence over template)
            var menuForDate = menus.Where(m => m.MenuDate.HasValue && m.MenuDate.Value.Date == att.Date.Date).ToList();
            if (!menuForDate.Any())
            {
                menuForDate = menus.Where(m => !m.MenuDate.HasValue && m.DayOfWeek == att.Date.DayOfWeek).ToList();
            }

            if (att.BreakfastPresent)
            {
                var breakfast = menuForDate.FirstOrDefault(m => m.MealType == Models.MealType.Breakfast);
                totalCost += breakfast.Price > 0 ? breakfast.Price : Constants.DefaultBreakfastRate;
            }

            if (att.LunchPresent)
            {
                var lunch = menuForDate.FirstOrDefault(m => m.MealType == Models.MealType.Lunch);
                totalCost += lunch.Price > 0 ? lunch.Price : Constants.DefaultLunchRate;
            }

            if (att.DinnerPresent)
            {
                var dinner = menuForDate.FirstOrDefault(m => m.MealType == Models.MealType.Dinner);
                totalCost += dinner.Price > 0 ? dinner.Price : Constants.DefaultDinnerRate;
            }
        }

        return Math.Round(totalCost, 2);
    }
}

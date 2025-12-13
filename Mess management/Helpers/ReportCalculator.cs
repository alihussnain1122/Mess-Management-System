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
}

using System.Globalization;

namespace MessManagement.Helpers;

public static class DateHelper
{
    public static DateTime GetStartOfMonth(int month, int year)
    {
        return new DateTime(year, month, 1);
    }

    public static DateTime GetEndOfMonth(int month, int year)
    {
        return new DateTime(year, month, DateTime.DaysInMonth(year, month));
    }

    public static DateTime GetStartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    public static DateTime GetEndOfWeek(DateTime date)
    {
        return GetStartOfWeek(date).AddDays(6);
    }

    public static string GetMonthName(int month)
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
    }

    public static string FormatDate(DateTime date, string format = "dd MMM yyyy")
    {
        return date.ToString(format);
    }

    public static string FormatDateTime(DateTime date, string format = "dd MMM yyyy HH:mm")
    {
        return date.ToString(format);
    }

    public static int GetDaysInMonth(int month, int year)
    {
        return DateTime.DaysInMonth(year, month);
    }

    public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
    {
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    public static bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    public static int GetWeekOfYear(DateTime date)
    {
        var cal = CultureInfo.CurrentCulture.Calendar;
        return cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
    }

    public static (int Month, int Year) GetCurrentMonthYear()
    {
        var now = DateTime.UtcNow;
        return (now.Month, now.Year);
    }

    public static (int Month, int Year) GetPreviousMonthYear()
    {
        var now = DateTime.UtcNow.AddMonths(-1);
        return (now.Month, now.Year);
    }
}

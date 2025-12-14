using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Services;

public class AttendanceService : IAttendanceService
{
    private readonly MessDbContext _context;
    private readonly IMemberService _memberService;

    public AttendanceService(MessDbContext context, IMemberService memberService)
    {
        _context = context;
        _memberService = memberService;
    }

    public async Task<IEnumerable<Attendance>> GetAttendanceForMemberAsync(int memberId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances
            .Include(a => a.MarkedByUser)
            .Where(a => a.MemberId == memberId);

        if (startDate.HasValue)
            query = query.Where(a => a.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(a => a.Date <= endDate.Value.Date);

        return await query.OrderByDescending(a => a.Date).ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetAttendanceForDateAsync(DateTime date)
    {
        return await _context.Attendances
            .Include(a => a.Member)
            .Include(a => a.MarkedByUser)
            .Where(a => a.Date.Date == date.Date)
            .OrderBy(a => a.Member.FullName)
            .ToListAsync();
    }

    public async Task<Attendance?> GetAttendanceByIdAsync(int id)
    {
        return await _context.Attendances
            .Include(a => a.Member)
            .Include(a => a.MarkedByUser)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Attendance?> GetAttendanceAsync(int memberId, DateTime date)
    {
        return await _context.Attendances
            .FirstOrDefaultAsync(a => a.MemberId == memberId && a.Date.Date == date.Date);
    }

    // New meal-wise attendance method
    public async Task<Attendance> MarkAttendanceAsync(int memberId, DateTime date, bool breakfast, bool lunch, bool dinner, int markedBy)
    {
        var existingAttendance = await GetAttendanceAsync(memberId, date);

        if (existingAttendance != null)
        {
            existingAttendance.BreakfastPresent = breakfast;
            existingAttendance.LunchPresent = lunch;
            existingAttendance.DinnerPresent = dinner;
            existingAttendance.MarkedBy = markedBy;
            existingAttendance.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingAttendance;
        }

        var attendance = new Attendance
        {
            MemberId = memberId,
            Date = date.Date,
            BreakfastPresent = breakfast,
            LunchPresent = lunch,
            DinnerPresent = dinner,
            MarkedBy = markedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();

        return attendance;
    }

    public async Task MarkAllAttendanceAsync(DateTime date, bool breakfast, bool lunch, bool dinner, int markedBy)
    {
        // Run queries sequentially to avoid DbContext concurrency issues
        var activeMembers = await _memberService.GetActiveMembersAsync();
        var existingAttendances = await _context.Attendances
            .Where(a => a.Date.Date == date.Date)
            .ToListAsync();
        var existingMemberIds = existingAttendances.Select(a => a.MemberId).ToHashSet();

        // Update existing attendance records
        foreach (var existing in existingAttendances)
        {
            existing.BreakfastPresent = breakfast;
            existing.LunchPresent = lunch;
            existing.DinnerPresent = dinner;
            existing.MarkedBy = markedBy;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        // Create new attendance records for members without one
        var newAttendances = activeMembers
            .Where(m => !existingMemberIds.Contains(m.MemberId))
            .Select(m => new Attendance
            {
                MemberId = m.MemberId,
                Date = date.Date,
                BreakfastPresent = breakfast,
                LunchPresent = lunch,
                DinnerPresent = dinner,
                MarkedBy = markedBy,
                CreatedAt = DateTime.UtcNow
            });

        _context.Attendances.AddRange(newAttendances);
        await _context.SaveChangesAsync();
    }

    // Legacy methods for backward compatibility
    public async Task<Attendance> MarkPresentAsync(int memberId, DateTime date, int markedBy)
    {
        return await MarkAttendanceAsync(memberId, date, true, true, true, markedBy);
    }

    public async Task<Attendance> MarkAbsentAsync(int memberId, DateTime date, int markedBy)
    {
        return await MarkAttendanceAsync(memberId, date, false, false, false, markedBy);
    }

    public async Task MarkAllPresentAsync(DateTime date, int markedBy)
    {
        await MarkAllAttendanceAsync(date, true, true, true, markedBy);
    }

    public async Task<Attendance> UpdateAttendanceAsync(int attendanceId, AttendanceStatus status, int markedBy)
    {
        var attendance = await _context.Attendances.FindAsync(attendanceId);

        if (attendance == null)
            throw new ArgumentException("Attendance record not found", nameof(attendanceId));

        // Update all meals based on status
        var isPresent = status == AttendanceStatus.Present;
        attendance.BreakfastPresent = isPresent;
        attendance.LunchPresent = isPresent;
        attendance.DinnerPresent = isPresent;
        attendance.MarkedBy = markedBy;
        attendance.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return attendance;
    }

    public async Task<bool> DeleteAttendanceAsync(int id)
    {
        var attendance = await _context.Attendances.FindAsync(id);

        if (attendance == null)
            return false;

        _context.Attendances.Remove(attendance);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<MonthlyAttendanceReport> GetMonthlyAttendanceReportAsync(int memberId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var attendances = await _context.Attendances
            .Where(a => a.MemberId == memberId && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        var member = await _memberService.GetMemberByIdAsync(memberId);

        // Count days where at least one meal was attended
        var presentDays = attendances.Count(a => a.BreakfastPresent || a.LunchPresent || a.DinnerPresent);
        var absentDays = attendances.Count(a => !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent);

        return new MonthlyAttendanceReport
        {
            MemberId = memberId,
            MemberName = member?.FullName ?? "Unknown",
            Month = month,
            Year = year,
            TotalDays = DateTime.DaysInMonth(year, month),
            PresentDays = presentDays,
            AbsentDays = absentDays,
            AttendanceRecords = attendances
        };
    }

    public async Task<MealWiseAttendanceSummary> GetMealWiseAttendanceSummaryAsync(int memberId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var attendances = await _context.Attendances
            .Where(a => a.MemberId == memberId && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        var member = await _memberService.GetMemberByIdAsync(memberId);

        return new MealWiseAttendanceSummary
        {
            MemberId = memberId,
            MemberName = member?.FullName ?? "Unknown",
            Month = month,
            Year = year,
            TotalDays = DateTime.DaysInMonth(year, month),
            BreakfastCount = attendances.Count(a => a.BreakfastPresent),
            LunchCount = attendances.Count(a => a.LunchPresent),
            DinnerCount = attendances.Count(a => a.DinnerPresent),
            AttendanceRecords = attendances
        };
    }

    public async Task<IEnumerable<AttendanceMismatch>> VerifyAttendanceMismatchAsync(int memberId, DateTime startDate, DateTime endDate)
    {
        var attendances = await GetAttendanceForMemberAsync(memberId, startDate, endDate);
        var mismatches = new List<AttendanceMismatch>();
        return mismatches;
    }

    public async Task<int> GetPresentCountForMemberAsync(int memberId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Count days where at least one meal was attended
        return await _context.Attendances
            .CountAsync(a => a.MemberId == memberId 
                && a.Date >= startDate 
                && a.Date <= endDate 
                && (a.BreakfastPresent || a.LunchPresent || a.DinnerPresent));
    }

    public async Task<int> GetAbsentCountForMemberAsync(int memberId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Count days where no meal was attended
        return await _context.Attendances
            .CountAsync(a => a.MemberId == memberId 
                && a.Date >= startDate 
                && a.Date <= endDate 
                && !a.BreakfastPresent && !a.LunchPresent && !a.DinnerPresent);
    }

    public async Task<MealCounts> GetMealCountsForMemberAsync(int memberId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var attendances = await _context.Attendances
            .Where(a => a.MemberId == memberId && a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        return new MealCounts
        {
            BreakfastCount = attendances.Count(a => a.BreakfastPresent),
            LunchCount = attendances.Count(a => a.LunchPresent),
            DinnerCount = attendances.Count(a => a.DinnerPresent)
        };
    }
}

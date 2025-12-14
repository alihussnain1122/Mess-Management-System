using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Services;

public class PaymentService : IPaymentService
{
    private readonly MessDbContext _context;
    private readonly IMemberService _memberService;
    private readonly IAttendanceService _attendanceService;
    private readonly IMenuService _menuService;

    public PaymentService(
        MessDbContext context, 
        IMemberService memberService,
        IAttendanceService attendanceService,
        IMenuService menuService)
    {
        _context = context;
        _memberService = memberService;
        _attendanceService = attendanceService;
        _menuService = menuService;
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _context.Payments
            .Include(p => p.Member)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsForMemberAsync(int memberId)
    {
        return await _context.Payments
            .Where(p => p.MemberId == memberId)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<Payment?> GetPaymentByIdAsync(int id)
    {
        return await _context.Payments
            .Include(p => p.Member)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Payment> AddPaymentAsync(Payment payment)
    {
        payment.Date = DateTime.UtcNow;
        payment.CreatedAt = DateTime.UtcNow;
        
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        
        return payment;
    }

    public async Task<Payment> UpdatePaymentAsync(Payment payment)
    {
        var existingPayment = await _context.Payments.FindAsync(payment.Id);
        
        if (existingPayment == null)
            throw new ArgumentException("Payment not found", nameof(payment));

        existingPayment.Amount = payment.Amount;
        existingPayment.PaymentMode = payment.PaymentMode;
        existingPayment.Notes = payment.Notes;
        existingPayment.Status = payment.Status;

        await _context.SaveChangesAsync();
        
        return existingPayment;
    }

    public async Task<bool> DeletePaymentAsync(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        
        if (payment == null)
            return false;

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<decimal> GetTotalPaymentsAsync(int memberId)
    {
        return await _context.Payments
            .Where(p => p.MemberId == memberId && p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);
    }

    public async Task<decimal> GetTotalPaymentsForPeriodAsync(int memberId, DateTime startDate, DateTime endDate)
    {
        return await _context.Payments
            .Where(p => p.MemberId == memberId 
                && p.Date >= startDate 
                && p.Date <= endDate 
                && p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);
    }

    public async Task<PaymentSummary> GetPaymentSummaryAsync(int memberId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var member = await _memberService.GetMemberByIdAsync(memberId);
        var payments = await _context.Payments
            .Where(p => p.MemberId == memberId && p.Date >= startDate && p.Date <= endDate)
            .ToListAsync();

        var presentDays = await _attendanceService.GetPresentCountForMemberAsync(memberId, month, year);
        
        // Calculate expected amount based on attendance
        var weeklyMenuCost = await _menuService.GetWeeklyMenuCostSummaryAsync();
        var averageDailyCost = weeklyMenuCost.Values.Average();
        var expectedAmount = presentDays * averageDailyCost;

        var totalPaid = payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);

        return new PaymentSummary
        {
            MemberId = memberId,
            MemberName = member?.FullName ?? "Unknown",
            Month = month,
            Year = year,
            TotalExpectedAmount = expectedAmount,
            TotalPaidAmount = totalPaid,
            BalanceAmount = expectedAmount - totalPaid,
            PaymentCount = payments.Count,
            Payments = payments
        };
    }

    public async Task<InvoiceViewModel> GenerateInvoiceAsync(int memberId, int month, int year)
    {
        var member = await _memberService.GetMemberByIdAsync(memberId);
        var paymentSummary = await GetPaymentSummaryAsync(memberId, month, year);
        var attendanceReport = await _attendanceService.GetMonthlyAttendanceReportAsync(memberId, month, year);

        return new InvoiceViewModel
        {
            InvoiceNumber = $"INV-{year}{month:D2}-{memberId:D4}",
            InvoiceDate = DateTime.UtcNow,
            MemberId = memberId,
            MemberName = member?.FullName ?? "Unknown",
            RoomNumber = member?.RoomNumber ?? "N/A",
            Month = month,
            Year = year,
            TotalPresentDays = attendanceReport.PresentDays,
            TotalAbsentDays = attendanceReport.AbsentDays,
            MealCharges = paymentSummary.TotalExpectedAmount,
            WaterTeaCharges = 0, // Calculate from WaterTea records
            TotalAmount = paymentSummary.TotalExpectedAmount,
            AmountPaid = paymentSummary.TotalPaidAmount,
            BalanceDue = paymentSummary.BalanceAmount,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Payments
            .Include(p => p.Member)
            .Where(p => p.Date >= startDate && p.Date <= endDate)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsForDateAsync(DateTime date)
    {
        return await _context.Payments
            .Include(p => p.Member)
            .Where(p => p.Date.Date == date.Date)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        return await _context.Payments
            .Where(p => p.Date >= startDate && p.Date <= endDate && p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);
    }

    public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
    {
        return await _context.Payments
            .Include(p => p.Member)
            .Where(p => p.Status == PaymentStatus.Pending)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<int> GetPendingPaymentsCountAsync()
    {
        return await _context.Payments
            .CountAsync(p => p.Status == PaymentStatus.Pending);
    }

    public async Task<bool> VerifyPaymentAsync(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null || payment.Status != PaymentStatus.Pending)
            return false;

        payment.Status = PaymentStatus.Completed;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectPaymentAsync(int id, string? reason = null)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null || payment.Status != PaymentStatus.Pending)
            return false;

        payment.Status = PaymentStatus.Failed;
        if (!string.IsNullOrEmpty(reason))
            payment.Notes = $"Rejected: {reason}";
        
        await _context.SaveChangesAsync();
        return true;
    }
}

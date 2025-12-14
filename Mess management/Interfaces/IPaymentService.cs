using MessManagement.Models;
using MessManagement.ViewModels;

namespace MessManagement.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    Task<IEnumerable<Payment>> GetPaymentsForMemberAsync(int memberId);
    Task<Payment?> GetPaymentByIdAsync(int id);
    Task<Payment> AddPaymentAsync(Payment payment);
    Task<Payment> UpdatePaymentAsync(Payment payment);
    Task<bool> DeletePaymentAsync(int id);
    Task<decimal> GetTotalPaymentsAsync(int memberId);
    Task<decimal> GetTotalPaymentsForPeriodAsync(int memberId, DateTime startDate, DateTime endDate);
    Task<PaymentSummary> GetPaymentSummaryAsync(int memberId, int month, int year);
    Task<InvoiceViewModel> GenerateInvoiceAsync(int memberId, int month, int year);
    Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Payment>> GetPaymentsForDateAsync(DateTime date);
    Task<decimal> GetTotalRevenueAsync(int month, int year);
    Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
    Task<int> GetPendingPaymentsCountAsync();
    Task<bool> VerifyPaymentAsync(int id);
    Task<bool> RejectPaymentAsync(int id, string? reason = null);
}

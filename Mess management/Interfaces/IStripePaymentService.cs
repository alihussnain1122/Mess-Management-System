using MessManagement.ViewModels;

namespace MessManagement.Interfaces;

public interface IStripePaymentService
{
    Task<StripePaymentIntent> CreatePaymentIntentAsync(int memberId, decimal amount, string description);
    Task<bool> HandleStripeWebhookAsync(string json, string signature);
    Task<StripeTransaction> SaveTransactionAsync(StripeTransaction transaction);
    Task<StripeTransaction?> GetTransactionByPaymentIntentIdAsync(string paymentIntentId);
    Task<bool> ConfirmPaymentAsync(string paymentIntentId);
    Task<bool> RefundPaymentAsync(string paymentIntentId, decimal? amount = null);
}

using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MessManagement.Services;

public class StripePaymentService : IStripePaymentService
{
    private readonly MessDbContext _context;
    private readonly StripeSettings _stripeSettings;
    private readonly IPaymentService _paymentService;

    public StripePaymentService(
        MessDbContext context,
        IOptions<StripeSettings> stripeSettings,
        IPaymentService paymentService)
    {
        _context = context;
        _stripeSettings = stripeSettings.Value;
        _paymentService = paymentService;
    }

    public async Task<StripePaymentIntent> CreatePaymentIntentAsync(int memberId, decimal amount, string description)
    {
        // In production, use Stripe.NET SDK
        // Stripe.StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        
        // var options = new PaymentIntentCreateOptions
        // {
        //     Amount = (long)(amount * 100), // Amount in cents
        //     Currency = "inr",
        //     Description = description,
        //     Metadata = new Dictionary<string, string>
        //     {
        //         { "MemberId", memberId.ToString() }
        //     }
        // };
        // var service = new PaymentIntentService();
        // var paymentIntent = await service.CreateAsync(options);

        // Simulated response for development
        var paymentIntent = new StripePaymentIntent
        {
            Id = $"pi_{Guid.NewGuid():N}",
            ClientSecret = $"pi_{Guid.NewGuid():N}_secret_{Guid.NewGuid():N}",
            Amount = amount,
            Currency = "inr",
            Status = "requires_payment_method",
            MemberId = memberId,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        return await Task.FromResult(paymentIntent);
    }

    public async Task<bool> HandleStripeWebhookAsync(string json, string signature)
    {
        try
        {
            // In production, verify webhook signature
            // var stripeEvent = EventUtility.ConstructEvent(json, signature, _stripeSettings.WebhookSecret);

            // Handle different event types
            // switch (stripeEvent.Type)
            // {
            //     case Events.PaymentIntentSucceeded:
            //         var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            //         await HandlePaymentSucceededAsync(paymentIntent);
            //         break;
            //     case Events.PaymentIntentPaymentFailed:
            //         // Handle failed payment
            //         break;
            // }

            return await Task.FromResult(true);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<StripeTransaction> SaveTransactionAsync(StripeTransaction transaction)
    {
        // Save transaction to database or log
        // This would typically be stored in a separate transactions table
        
        if (transaction.Status == "succeeded")
        {
            var payment = new Payment
            {
                MemberId = transaction.MemberId,
                Amount = transaction.Amount,
                PaymentMode = PaymentMode.Online,
                StripePaymentId = transaction.PaymentIntentId,
                Status = PaymentStatus.Completed,
                Notes = transaction.Description
            };

            await _paymentService.AddPaymentAsync(payment);
        }

        return await Task.FromResult(transaction);
    }

    public async Task<StripeTransaction?> GetTransactionByPaymentIntentIdAsync(string paymentIntentId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.StripePaymentId == paymentIntentId);

        if (payment == null)
            return null;

        return new StripeTransaction
        {
            PaymentIntentId = paymentIntentId,
            MemberId = payment.MemberId,
            Amount = payment.Amount,
            Status = payment.Status.ToString(),
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<bool> ConfirmPaymentAsync(string paymentIntentId)
    {
        // In production, confirm payment with Stripe
        // var service = new PaymentIntentService();
        // var paymentIntent = await service.ConfirmAsync(paymentIntentId);

        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.StripePaymentId == paymentIntentId);

        if (payment == null)
            return false;

        payment.Status = PaymentStatus.Completed;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RefundPaymentAsync(string paymentIntentId, decimal? amount = null)
    {
        // In production, create refund with Stripe
        // var options = new RefundCreateOptions
        // {
        //     PaymentIntent = paymentIntentId,
        //     Amount = amount.HasValue ? (long)(amount.Value * 100) : null
        // };
        // var service = new RefundService();
        // var refund = await service.CreateAsync(options);

        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.StripePaymentId == paymentIntentId);

        if (payment == null)
            return false;

        payment.Status = PaymentStatus.Refunded;
        await _context.SaveChangesAsync();

        return true;
    }
}

public class StripeSettings
{
    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

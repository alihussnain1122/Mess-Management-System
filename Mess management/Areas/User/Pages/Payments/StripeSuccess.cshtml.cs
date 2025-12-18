using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;

namespace MessManagement.Areas.User.Pages.Payments;

[Authorize(Roles = "User")]
public class StripeSuccessModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private readonly IMemberService _memberService;
    private readonly StripeSettings _stripeSettings;

    public StripeSuccessModel(
        IPaymentService paymentService, 
        IMemberService memberService,
        IOptions<StripeSettings> stripeSettings)
    {
        _paymentService = paymentService;
        _memberService = memberService;
        _stripeSettings = stripeSettings.Value;
    }

    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public bool PaymentRecorded { get; set; }

    public async Task<IActionResult> OnGetAsync(string session_id)
    {
        if (string.IsNullOrEmpty(session_id))
        {
            TempData["ToastError"] = "Invalid payment session";
            return RedirectToPage("PayNow");
        }

        try
        {
            // Configure Stripe
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            // Retrieve the session to verify payment
            var service = new SessionService();
            Session session = await service.GetAsync(session_id);

            if (session.PaymentStatus != "paid")
            {
                TempData["ToastError"] = "Payment was not completed";
                return RedirectToPage("PayNow");
            }

            // Get member info from session metadata
            var memberId = int.Parse(session.Metadata["MemberId"]);
            MemberName = session.Metadata["MemberName"];
            Amount = decimal.Parse(session.Metadata["Amount"]);
            TransactionId = session.PaymentIntentId ?? session.Id;
            PaymentDate = DateTime.Now;

            // Check if payment already recorded (to prevent duplicates)
            var existingPayments = await _paymentService.GetPaymentsForMemberAsync(memberId);
            var alreadyRecorded = existingPayments.Any(p => p.StripePaymentId == TransactionId);

            if (!alreadyRecorded)
            {
                // Record payment in database
                var payment = new Payment
                {
                    MemberId = memberId,
                    Amount = Amount,
                    PaymentMode = PaymentMode.Online,
                    StripePaymentId = TransactionId,
                    Status = PaymentStatus.Completed, // Auto-verified since Stripe confirmed it
                    Date = DateTime.Now,
                    Notes = $"Stripe Payment - Session: {session_id}"
                };

                await _paymentService.AddPaymentAsync(payment);
                PaymentRecorded = true;
            }
            else
            {
                PaymentRecorded = true; // Already recorded from a previous visit
            }

            return Page();
        }
        catch (StripeException ex)
        {
            TempData["ToastError"] = $"Payment verification failed: {ex.Message}";
            return RedirectToPage("PayNow");
        }
        catch (Exception)
        {
            TempData["ToastError"] = "An error occurred while processing your payment";
            return RedirectToPage("PayNow");
        }
    }
}

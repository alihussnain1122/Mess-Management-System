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
public class StripeCheckoutModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private readonly IAttendanceService _attendanceService;
    private readonly IMemberService _memberService;
    private readonly StripeSettings _stripeSettings;
    private readonly IConfiguration _configuration;

    public StripeCheckoutModel(
        IPaymentService paymentService, 
        IAttendanceService attendanceService, 
        IMemberService memberService,
        IOptions<StripeSettings> stripeSettings,
        IConfiguration configuration)
    {
        _paymentService = paymentService;
        _attendanceService = attendanceService;
        _memberService = memberService;
        _stripeSettings = stripeSettings.Value;
        _configuration = configuration;
    }

    public decimal MonthCost { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string PublishableKey { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public decimal? Amount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadDataAsync();
        PublishableKey = _stripeSettings.PublishableKey;
        
        // If amount passed via query, use it
        if (!Amount.HasValue || Amount <= 0)
        {
            Amount = BalanceDue > 0 ? BalanceDue : 100;
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostCreateCheckoutSessionAsync(decimal amount)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return BadRequest("Invalid user");

        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member == null) 
            return BadRequest("Member not found");

        // Configure Stripe
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

        // Get base URL for redirect
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100), // Amount in paisa (smallest currency unit)
                        Currency = "pkr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Mess Payment",
                            Description = $"Monthly mess dues payment for {member.FullName}",
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = $"{baseUrl}/User/Payments/StripeSuccess?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{baseUrl}/User/Payments/StripeCancel",
            CustomerEmail = member.User?.Email,
            Metadata = new Dictionary<string, string>
            {
                { "MemberId", member.MemberId.ToString() },
                { "MemberName", member.FullName },
                { "Amount", amount.ToString() }
            }
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return new JsonResult(new { sessionId = session.Id, url = session.Url });
    }

    private async Task LoadDataAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return;

        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member == null) return;

        MemberName = member.FullName;
        var now = DateTime.Now;
        
        var presentDays = await _attendanceService.GetPresentCountForMemberAsync(member.MemberId, now.Month, now.Year);
        var payments = await _paymentService.GetPaymentsForMemberAsync(member.MemberId);

        TotalPaid = payments.Where(p => p.Date.Month == now.Month && p.Date.Year == now.Year && p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
        MonthCost = presentDays * 150m;
        BalanceDue = Math.Max(0, MonthCost - TotalPaid);
    }
}

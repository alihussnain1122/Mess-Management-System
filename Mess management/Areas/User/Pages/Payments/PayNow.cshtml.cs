using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MessManagement.Areas.User.Pages.Payments;

[Authorize(Roles = "User")]
public class PayNowModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private readonly IAttendanceService _attendanceService;
    private readonly IMemberService _memberService;

    public PayNowModel(IPaymentService paymentService, IAttendanceService attendanceService, IMemberService memberService)
    {
        _paymentService = paymentService;
        _attendanceService = attendanceService;
        _memberService = memberService;
    }

    public decimal MonthCost { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostAsync(decimal Amount, string PaymentMethod)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Page();

        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member == null) return Page();

        var payment = new Payment
        {
            MemberId = member.MemberId,
            Amount = Amount,
            PaymentMode = PaymentMethod == "Cash" ? PaymentMode.Cash : PaymentMode.Online,
            Date = DateTime.Now,
            Status = PaymentStatus.Pending // Payment requires admin verification
        };

        await _paymentService.AddPaymentAsync(payment);
        
        TempData["Success"] = "Payment submitted successfully! It will be verified by admin shortly.";
        return RedirectToPage("History");
    }

    private async Task LoadDataAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return;

        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member == null) return;

        var now = DateTime.Now;
        
        // Run queries sequentially to avoid DbContext concurrency issues
        var presentDays = await _attendanceService.GetPresentCountForMemberAsync(member.MemberId, now.Month, now.Year);
        var payments = await _paymentService.GetPaymentsForMemberAsync(member.MemberId);

        TotalPaid = payments.Where(p => p.Date.Month == now.Month && p.Date.Year == now.Year).Sum(p => p.Amount);
        MonthCost = presentDays * 150m;
        BalanceDue = Math.Max(0, MonthCost - TotalPaid);
    }
}
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MessManagement.Areas.User.Pages.Payments;

[Authorize(Roles = "User")]
public class HistoryModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private readonly IMemberService _memberService;
    private const int PageSize = 10;

    public HistoryModel(IPaymentService paymentService, IMemberService memberService)
    {
        _paymentService = paymentService;
        _memberService = memberService;
    }

    public PaginatedList<Payment>? Payments { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal ThisMonthPaid { get; set; }
    public int TransactionCount { get; set; }

    public async Task OnGetAsync(int pageIndex = 1)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return;

        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member == null) return;

        var allPayments = (await _paymentService.GetPaymentsForMemberAsync(member.MemberId))
            .OrderByDescending(p => p.Date);

        TotalPaid = allPayments.Sum(p => p.Amount);
        ThisMonthPaid = allPayments
            .Where(p => p.Date.Month == DateTime.Now.Month && p.Date.Year == DateTime.Now.Year)
            .Sum(p => p.Amount);
        TransactionCount = allPayments.Count();

        Payments = PaginatedList<Payment>.Create(allPayments, pageIndex, PageSize, null);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Payments;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private const int PageSize = 10;

    public IndexModel(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public PaginatedList<Payment>? Payments { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal CashTotal { get; set; }
    public decimal OnlineTotal { get; set; }
    public int PendingCount { get; set; }
    public string? SearchQuery { get; set; }

    public async Task OnGetAsync(int pageIndex = 1, string? search = null)
    {
        SearchQuery = search;
        var allPayments = await _paymentService.GetAllPaymentsAsync();
        var paymentsList = allPayments.ToList();

        // Calculate totals from all payments
        TotalCollected = paymentsList.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
        CashTotal = paymentsList.Where(p => p.PaymentMode == PaymentMode.Cash && p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
        OnlineTotal = paymentsList.Where(p => p.PaymentMode == PaymentMode.Online && p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
        PendingCount = paymentsList.Count(p => p.Status == PaymentStatus.Pending);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            paymentsList = paymentsList.Where(p =>
                (p.Member?.FullName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                p.Amount.ToString().Contains(search) ||
                p.PaymentMode.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Status.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        Payments = PaginatedList<Payment>.Create(paymentsList, pageIndex, PageSize, search);
    }

    public async Task<IActionResult> OnPostVerifyAsync(int id)
    {
        var result = await _paymentService.VerifyPaymentAsync(id);
        if (result)
            TempData["Success"] = "Payment verified successfully!";
        else
            TempData["Error"] = "Failed to verify payment.";
        
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id, string? reason)
    {
        var result = await _paymentService.RejectPaymentAsync(id, reason);
        if (result)
            TempData["Success"] = "Payment rejected.";
        else
            TempData["Error"] = "Failed to reject payment.";
        
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(int id, string status)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
        {
            TempData["Error"] = "Payment not found.";
            return RedirectToPage();
        }

        if (Enum.TryParse<PaymentStatus>(status, out var newStatus))
        {
            payment.Status = newStatus;
            await _paymentService.UpdatePaymentAsync(payment);
            TempData["Success"] = $"Payment status changed to {newStatus}.";
        }
        else
        {
            TempData["Error"] = "Invalid status.";
        }
        
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _paymentService.DeletePaymentAsync(id);
        if (result)
            TempData["Success"] = "Payment deleted successfully!";
        else
            TempData["Error"] = "Failed to delete payment.";
        
        return RedirectToPage();
    }
}
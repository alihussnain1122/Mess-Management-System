using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Payments;

[Authorize(Roles = "Admin")]
public class HistoryModel : PageModel
{
    private readonly IPaymentService _paymentService;

    public HistoryModel(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
    public DateTime EndDate { get; set; } = DateTime.Today;
    public IEnumerable<Payment>? Payments { get; set; }

    public async Task OnGetAsync(DateTime? startDate, DateTime? endDate)
    {
        StartDate = startDate ?? DateTime.Today.AddDays(-30);
        EndDate = endDate ?? DateTime.Today;

        Payments = await _paymentService.GetPaymentsByDateRangeAsync(StartDate, EndDate);
    }
}
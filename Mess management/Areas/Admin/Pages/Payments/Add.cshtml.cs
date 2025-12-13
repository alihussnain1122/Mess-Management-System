using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MessManagement.Areas.Admin.Pages.Payments;

[Authorize(Roles = "Admin")]
public class AddModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private readonly IMemberService _memberService;
    private readonly IEmailService _emailService;

    public AddModel(IPaymentService paymentService, IMemberService memberService, IEmailService emailService)
    {
        _paymentService = paymentService;
        _memberService = memberService;
        _emailService = emailService;
    }

    [BindProperty]
    public AddPaymentViewModel Input { get; set; } = new();

    public SelectList? MemberList { get; set; }

    public async Task OnGetAsync()
    {
        await LoadMembersAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadMembersAsync();
            return Page();
        }

        var payment = new Payment
        {
            MemberId = Input.MemberId,
            Amount = Input.Amount,
            PaymentMode = Input.PaymentMode,
            Notes = Input.Notes,
            Status = PaymentStatus.Completed
        };

        await _paymentService.AddPaymentAsync(payment);

        // Send payment confirmation email
        var member = await _memberService.GetMemberByIdAsync(Input.MemberId);
        if (member?.User?.Email != null)
        {
            await _emailService.SendPaymentConfirmationAsync(
                member.User.Email, 
                member.FullName, 
                Input.Amount, 
                Input.PaymentMode.ToString());
        }

        TempData["ToastSuccess"] = $"Payment of Rs.{Input.Amount:N2} recorded successfully!";
        return RedirectToPage("Index");
    }

    private async Task LoadMembersAsync()
    {
        var members = await _memberService.GetActiveMembersAsync();
        MemberList = new SelectList(members, "MemberId", "FullName");
    }
}
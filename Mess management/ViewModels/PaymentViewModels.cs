using System.ComponentModel.DataAnnotations;
using MessManagement.Models;

namespace MessManagement.ViewModels;

public class PaymentSummary
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalExpectedAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public int PaymentCount { get; set; }
    public IEnumerable<Payment> Payments { get; set; } = new List<Payment>();
}

public class InvoiceViewModel
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalPresentDays { get; set; }
    public int TotalAbsentDays { get; set; }
    public decimal MealCharges { get; set; }
    public decimal WaterTeaCharges { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class AddPaymentViewModel
{
    [Required(ErrorMessage = "Please select a member")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid member")]
    public int MemberId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(1, 100000, ErrorMessage = "Amount must be between Rs.1 and Rs.100,000")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment mode is required")]
    public PaymentMode PaymentMode { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}

public class StripePaymentIntent
{
    public string Id { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "inr";
    public string Status { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StripeTransaction
{
    public string PaymentIntentId { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PayNowViewModel
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceDue { get; set; }
    public string StripePublishableKey { get; set; } = string.Empty;
    public string? ClientSecret { get; set; }
}

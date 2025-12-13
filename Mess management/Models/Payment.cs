using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessManagement.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MemberId { get; set; }

    [ForeignKey(nameof(MemberId))]
    public Member Member { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;

    [StringLength(200)]
    public string? StripePaymentId { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum PaymentMode
{
    Cash,
    Online
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

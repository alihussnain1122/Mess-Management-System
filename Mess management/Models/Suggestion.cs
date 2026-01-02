using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessManagement.Models;

public class Suggestion
{
    [Key]
    public int SuggestionId { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public SuggestionCategory Category { get; set; } = SuggestionCategory.General;

    [Required]
    public SuggestionStatus Status { get; set; } = SuggestionStatus.Pending;

    public SuggestionPriority Priority { get; set; } = SuggestionPriority.Normal;

    // Admin Response
    [StringLength(1000)]
    public string? AdminResponse { get; set; }

    public int? RespondedByUserId { get; set; }

    public DateTime? RespondedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Flag for anonymous suggestions
    public bool IsAnonymous { get; set; } = false;

    // Navigation Properties
    [ForeignKey("MemberId")]
    public virtual Member? Member { get; set; }

    [ForeignKey("RespondedByUserId")]
    public virtual User? RespondedByUser { get; set; }
}

public enum SuggestionCategory
{
    General,
    Menu,
    Quality,
    Service,
    Hygiene,
    Pricing,
    Timing,
    Staff,
    Complaint,
    Appreciation,
    Other
}

public enum SuggestionStatus
{
    Pending,
    UnderReview,
    Resolved,
    Rejected,
    Implemented
}

public enum SuggestionPriority
{
    Low,
    Normal,
    High,
    Urgent
}

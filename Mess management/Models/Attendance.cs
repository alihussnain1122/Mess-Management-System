using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessManagement.Models;

public class Attendance
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MemberId { get; set; }

    [ForeignKey(nameof(MemberId))]
    public Member Member { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    // Meal-wise attendance tracking
    public bool BreakfastPresent { get; set; } = true;
    public bool LunchPresent { get; set; } = true;
    public bool DinnerPresent { get; set; } = true;

    // Legacy status - computed for backward compatibility (not mapped to DB)
    [NotMapped]
    public AttendanceStatus Status
    {
        get => (BreakfastPresent || LunchPresent || DinnerPresent) 
            ? AttendanceStatus.Present 
            : AttendanceStatus.Absent;
        set { /* no-op setter for EF compatibility */ }
    }

    // Count of meals attended (not mapped to DB)
    [NotMapped]
    public int MealsAttended => 
        (BreakfastPresent ? 1 : 0) + 
        (LunchPresent ? 1 : 0) + 
        (DinnerPresent ? 1 : 0);

    public int MarkedBy { get; set; }

    [ForeignKey(nameof(MarkedBy))]
    public User MarkedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum AttendanceStatus
{
    Present,
    Absent
}

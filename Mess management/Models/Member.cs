using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessManagement.Models;

public class Member
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    public DateTime JoinDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Concurrency token for optimistic concurrency control
    // EF Core will check this value during updates to prevent lost updates
    [Timestamp]
    public byte[]? RowVersion { get; set; }

    // Foreign Key
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    // Navigation properties
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<WaterTea> WaterTeaRecords { get; set; } = new List<WaterTea>();
}

using System.ComponentModel.DataAnnotations;

namespace MessManagement.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string PasswordSalt { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.User;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ============================================
    // ACCOUNT LOCKOUT PROPERTIES (Security Feature)
    // ============================================
    
    /// <summary>
    /// Number of consecutive failed login attempts
    /// </summary>
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// UTC time when the account lockout expires (null = not locked)
    /// </summary>
    public DateTime? LockoutEnd { get; set; }

    /// <summary>
    /// Whether lockout is enabled for this account
    /// </summary>
    public bool LockoutEnabled { get; set; } = true;

    /// <summary>
    /// Last successful login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    // Navigation property
    public Member? Member { get; set; }

    // ============================================
    // HELPER METHODS
    // ============================================

    /// <summary>
    /// Check if the account is currently locked out
    /// </summary>
    public bool IsLockedOut => LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

    /// <summary>
    /// Get remaining lockout time
    /// </summary>
    public TimeSpan? RemainingLockoutTime => IsLockedOut ? LockoutEnd!.Value - DateTime.UtcNow : null;
}

public enum UserRole
{
    Admin,
    User
}

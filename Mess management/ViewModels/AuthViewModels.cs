using System.ComponentModel.DataAnnotations;
using MessManagement.Models;
using MessManagement.Helpers; // For custom validation attributes

namespace MessManagement.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50)]
    [NoSpecialCharacters("_", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3)]
    [NoSpecialCharacters("_", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [StrongPassword(MinLength = 6, RequireUppercase = true, RequireLowercase = true, RequireDigit = true)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100)]
    [NoSpecialCharacters(" -'", ErrorMessage = "Full Name contains invalid characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room Number is required")]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current Password is required")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New Password is required")]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserProfileViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public MemberProfileViewModel? Member { get; set; }
}

public class MemberProfileViewModel
{
    public int MemberId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace MessManagement.ViewModels;

public class MemberViewModel
{
    public int MemberId { get; set; }

    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room Number is required")]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class AddMemberViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room Number is required")]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;
}

public class EditMemberViewModel
{
    public int MemberId { get; set; }

    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room Number is required")]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace MessManagement.Helpers;

/// <summary>
/// Custom validation attribute that ensures a date is not in the past.
/// Demonstrates custom validation for EAD requirements.
/// </summary>
public class FutureDateAttribute : ValidationAttribute
{
    public bool AllowToday { get; set; } = true;

    public FutureDateAttribute() : base("Date cannot be in the past")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success; // Let [Required] handle null

        if (value is DateTime date)
        {
            var compareDate = AllowToday ? DateTime.Today.AddDays(-1) : DateTime.Today;
            
            if (date.Date <= compareDate)
            {
                var errorMessage = ErrorMessage ?? $"{validationContext.DisplayName} cannot be in the past";
                return new ValidationResult(errorMessage, new[] { validationContext.MemberName! });
            }
        }
        else if (value is DateOnly dateOnly)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var compareDate = AllowToday ? today.AddDays(-1) : today;
            
            if (dateOnly <= compareDate)
            {
                var errorMessage = ErrorMessage ?? $"{validationContext.DisplayName} cannot be in the past";
                return new ValidationResult(errorMessage, new[] { validationContext.MemberName! });
            }
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// Custom validation attribute that ensures a string does not contain special characters.
/// Useful for usernames, names, etc.
/// </summary>
public class NoSpecialCharactersAttribute : ValidationAttribute
{
    private readonly string _allowedSpecialChars;

    public NoSpecialCharactersAttribute(string allowedSpecialChars = "")
        : base("Field contains invalid characters")
    {
        _allowedSpecialChars = allowedSpecialChars;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        var input = value.ToString()!;
        
        foreach (char c in input)
        {
            if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && !_allowedSpecialChars.Contains(c))
            {
                var errorMessage = ErrorMessage ?? $"{validationContext.DisplayName} contains invalid characters";
                return new ValidationResult(errorMessage, new[] { validationContext.MemberName! });
            }
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// Custom validation attribute that validates password strength.
/// Requires at least one uppercase, one lowercase, one digit, and one special character.
/// </summary>
public class StrongPasswordAttribute : ValidationAttribute
{
    public int MinLength { get; set; } = 6;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireSpecialChar { get; set; } = false;

    public StrongPasswordAttribute() : base("Password does not meet strength requirements")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success; // Let [Required] handle null

        var password = value.ToString()!;
        var errors = new List<string>();

        if (password.Length < MinLength)
            errors.Add($"at least {MinLength} characters");

        if (RequireUppercase && !password.Any(char.IsUpper))
            errors.Add("one uppercase letter");

        if (RequireLowercase && !password.Any(char.IsLower))
            errors.Add("one lowercase letter");

        if (RequireDigit && !password.Any(char.IsDigit))
            errors.Add("one digit");

        if (RequireSpecialChar && password.All(c => char.IsLetterOrDigit(c)))
            errors.Add("one special character");

        if (errors.Any())
        {
            var errorMessage = $"{validationContext.DisplayName} must contain {string.Join(", ", errors)}";
            return new ValidationResult(errorMessage, new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success;
    }
}

namespace MessManagement.Helpers;

public static class Constants
{
    // Pricing
    public const decimal DefaultWaterCost = 5.00m;
    public const decimal DefaultTeaCost = 10.00m;
    
    // Meal Rates (per meal)
    public const decimal DefaultBreakfastRate = 50.00m;
    public const decimal DefaultLunchRate = 100.00m;
    public const decimal DefaultDinnerRate = 100.00m;

    // Pagination
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    // Session Keys
    public const string SessionKeyUserId = "UserId";
    public const string SessionKeyUsername = "Username";
    public const string SessionKeyRole = "Role";
    public const string SessionKeyMemberId = "MemberId";

    // Cookie Names
    public const string AuthCookieName = "MessAuth";

    // Claim Types
    public const string ClaimTypeUserId = "UserId";
    public const string ClaimTypeMemberId = "MemberId";
    public const string ClaimTypeRole = "Role";

    // Roles
    public const string RoleAdmin = "Admin";
    public const string RoleUser = "User";

    // Date Formats
    public const string DateFormat = "yyyy-MM-dd";
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const string DisplayDateFormat = "dd MMM yyyy";
    public const string DisplayDateTimeFormat = "dd MMM yyyy HH:mm";

    // Error Messages
    public static class ErrorMessages
    {
        public const string InvalidCredentials = "Invalid username or password.";
        public const string UserNotFound = "User not found.";
        public const string MemberNotFound = "Member not found.";
        public const string UnauthorizedAccess = "You are not authorized to access this resource.";
        public const string InvalidRequest = "Invalid request.";
        public const string OperationFailed = "Operation failed. Please try again.";
        public const string PaymentFailed = "Payment processing failed.";
    }

    // Success Messages
    public static class SuccessMessages
    {
        public const string LoginSuccess = "Login successful!";
        public const string LogoutSuccess = "You have been logged out.";
        public const string MemberAdded = "Member added successfully!";
        public const string MemberUpdated = "Member updated successfully!";
        public const string MemberDisabled = "Member has been disabled.";
        public const string AttendanceMarked = "Attendance marked successfully!";
        public const string PaymentAdded = "Payment recorded successfully!";
        public const string MenuUpdated = "Menu updated successfully!";
    }
}

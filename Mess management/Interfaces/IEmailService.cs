namespace MessManagement.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendPaymentReminderAsync(string toEmail, string memberName, decimal amount, DateTime dueDate);
    Task SendPaymentConfirmationAsync(string toEmail, string memberName, decimal amount, string paymentMode);
    Task SendWelcomeEmailAsync(string toEmail, string memberName, string username);
    Task SendPasswordResetCodeAsync(string toEmail, string userName, string code);
    Task SendMonthlyBillAsync(string toEmail, string memberName, MonthlyBillEmailModel bill);
}

public class MonthlyBillEmailModel
{
    public string MemberName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public int Month { get; set; }
    public int Year { get; set; }
    public string MonthName { get; set; } = "";
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    
    // Meal-wise Counts
    public int BreakfastCount { get; set; }
    public int LunchCount { get; set; }
    public int DinnerCount { get; set; }
    
    // Meal-wise Rates
    public decimal BreakfastRate { get; set; }
    public decimal LunchRate { get; set; }
    public decimal DinnerRate { get; set; }
    
    // Meal-wise Charges
    public decimal BreakfastCharges { get; set; }
    public decimal LunchCharges { get; set; }
    public decimal DinnerCharges { get; set; }
    
    // Legacy Total
    public decimal MealRate { get; set; }
    public decimal MealCharges { get; set; }
    
    // Water/Tea
    public int WaterCount { get; set; }
    public decimal WaterRate { get; set; }
    public decimal WaterCharges { get; set; }
    public int TeaCount { get; set; }
    public decimal TeaRate { get; set; }
    public decimal TeaCharges { get; set; }
    
    // Totals
    public decimal GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
}

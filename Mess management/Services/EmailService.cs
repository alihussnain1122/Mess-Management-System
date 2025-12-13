using MessManagement.Interfaces;
using System.Net;
using System.Net.Mail;

namespace MessManagement.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:Username"] ?? "";
            var smtpPass = _configuration["Email:Password"] ?? "";
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
            var fromName = _configuration["Email:FromName"] ?? "Mess Management System";

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                _logger.LogWarning("Email not configured. Skipping email to {Email}", toEmail);
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
        }
    }

    public async Task SendPaymentReminderAsync(string toEmail, string memberName, decimal amount, DateTime dueDate)
    {
        var subject = "Payment Reminder - Mess Management";
        var body = GetEmailTemplate("Payment Reminder", $@"
            <p>Dear <strong>{memberName}</strong>,</p>
            <p>This is a friendly reminder that your mess payment of <strong>Rs.{amount:N2}</strong> is due on <strong>{dueDate:dd MMM yyyy}</strong>.</p>
            <p>Please make the payment at your earliest convenience to avoid any inconvenience.</p>
            <div style='margin: 20px 0; padding: 15px; background: #fef3c7; border-radius: 8px; border-left: 4px solid #f59e0b;'>
                <p style='margin: 0; color: #92400e;'><strong>Amount Due:</strong> Rs.{amount:N2}</p>
                <p style='margin: 5px 0 0 0; color: #92400e;'><strong>Due Date:</strong> {dueDate:dd MMM yyyy}</p>
            </div>
            <p>Thank you for your cooperation.</p>
        ");

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPaymentConfirmationAsync(string toEmail, string memberName, decimal amount, string paymentMode)
    {
        var subject = "Payment Confirmation - Mess Management";
        var body = GetEmailTemplate("Payment Confirmed", $@"
            <p>Dear <strong>{memberName}</strong>,</p>
            <p>Your payment has been successfully received. Here are the details:</p>
            <div style='margin: 20px 0; padding: 15px; background: #d1fae5; border-radius: 8px; border-left: 4px solid #10b981;'>
                <p style='margin: 0; color: #065f46;'><strong>Amount Paid:</strong> Rs.{amount:N2}</p>
                <p style='margin: 5px 0 0 0; color: #065f46;'><strong>Payment Mode:</strong> {paymentMode}</p>
                <p style='margin: 5px 0 0 0; color: #065f46;'><strong>Date:</strong> {DateTime.Now:dd MMM yyyy, hh:mm tt}</p>
            </div>
            <p>Thank you for your payment!</p>
        ");

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string memberName, string username)
    {
        var subject = "Welcome to Mess Management System";
        var body = GetEmailTemplate("Welcome!", $@"
            <p>Dear <strong>{memberName}</strong>,</p>
            <p>Welcome to our Mess Management System! Your account has been created successfully.</p>
            <div style='margin: 20px 0; padding: 15px; background: #dbeafe; border-radius: 8px; border-left: 4px solid #3b82f6;'>
                <p style='margin: 0; color: #1e40af;'><strong>Username:</strong> {username}</p>
                <p style='margin: 5px 0 0 0; color: #1e40af;'>You can now log in to view your attendance, menu, and payment history.</p>
            </div>
            <p>If you have any questions, please contact the mess administrator.</p>
            <p>Best regards,<br>Mess Management Team</p>
        ");

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPasswordResetCodeAsync(string toEmail, string userName, string code)
    {
        var subject = "Password Reset Code - Mess Management";
        var body = GetEmailTemplate("Password Reset", $@"
            <p>Dear <strong>{userName}</strong>,</p>
            <p>We received a request to reset your password. Use the verification code below to proceed:</p>
            <div style='margin: 20px 0; padding: 25px; background: linear-gradient(135deg, #fef3c7, #fde68a); border-radius: 12px; text-align: center;'>
                <p style='margin: 0; color: #92400e; font-size: 14px;'>Your Verification Code</p>
                <p style='margin: 10px 0 0 0; color: #78350f; font-size: 36px; font-weight: bold; letter-spacing: 8px;'>{code}</p>
            </div>
            <p style='color: #dc2626;'><strong>⚠️ This code will expire in 15 minutes.</strong></p>
            <p>If you didn't request a password reset, please ignore this email or contact the administrator if you have concerns.</p>
            <p>Best regards,<br>Mess Management Team</p>
        ");

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendMonthlyBillAsync(string toEmail, string memberName, MonthlyBillEmailModel bill)
    {
        var subject = $"Monthly Bill - {bill.MonthName} {bill.Year} | Mess Management";
        
        var statusColor = bill.Balance > 0 ? "#dc2626" : "#10b981";
        var statusBg = bill.Balance > 0 ? "#fef2f2" : "#d1fae5";
        var statusText = bill.Balance > 0 ? "Balance Due" : "Fully Paid";
        
        // Calculate total meals for display
        var totalMeals = bill.BreakfastCount + bill.LunchCount + bill.DinnerCount;
        
        var body = GetEmailTemplate($"Monthly Bill - {bill.MonthName} {bill.Year}", $@"
            <p>Dear <strong>{memberName}</strong>,</p>
            <p>Here is your monthly bill statement for <strong>{bill.MonthName} {bill.Year}</strong>.</p>
            
            <!-- Member Info -->
            <div style='margin: 20px 0; padding: 15px; background: #f3f4f6; border-radius: 8px;'>
                <p style='margin: 0; color: #374151;'><strong>Member:</strong> {bill.MemberName}</p>
                <p style='margin: 5px 0 0 0; color: #374151;'><strong>Room Number:</strong> {bill.RoomNumber}</p>
            </div>

            <!-- Meal-wise Attendance Summary -->
            <h3 style='color: #374151; margin-top: 25px;'>🍽️ Meal Attendance Summary</h3>
            <table style='width: 100%; border-collapse: collapse; margin: 10px 0;'>
                <tr>
                    <td style='padding: 10px; background: #fef3c7; text-align: center; border-radius: 8px 0 0 0;'>
                        <span style='color: #92400e;'>☀️ Breakfast</span><br>
                        <strong style='font-size: 18px; color: #b45309;'>{bill.BreakfastCount}</strong>
                    </td>
                    <td style='padding: 10px; background: #dbeafe; text-align: center;'>
                        <span style='color: #1e40af;'>🌤️ Lunch</span><br>
                        <strong style='font-size: 18px; color: #1d4ed8;'>{bill.LunchCount}</strong>
                    </td>
                    <td style='padding: 10px; background: #e9d5ff; text-align: center; border-radius: 0 8px 0 0;'>
                        <span style='color: #7c3aed;'>🌙 Dinner</span><br>
                        <strong style='font-size: 18px; color: #6d28d9;'>{bill.DinnerCount}</strong>
                    </td>
                </tr>
            </table>

            <!-- Auto-Included Note -->
            <div style='margin: 15px 0; padding: 12px; background: linear-gradient(90deg, #ecfeff, #fef3c7); border-radius: 8px; border: 1px dashed #06b6d4;'>
                <p style='margin: 0; color: #0e7490; font-size: 13px;'>
                    <strong>ℹ️ Included with every meal:</strong> 💧 Water (FREE) + ☕ Tea/Coffee (Rs.100)
                </p>
            </div>

            <!-- Charges Breakdown -->
            <h3 style='color: #374151; margin-top: 25px;'>💰 Charges Breakdown</h3>
            <table style='width: 100%; border-collapse: collapse; margin: 10px 0;'>
                <tr style='background: #fef3c7;'>
                    <td style='padding: 12px; border: 1px solid #e5e7eb;'>☀️ Breakfast</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; color: #6b7280;'>{bill.BreakfastCount} × Rs.{bill.BreakfastRate}</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; font-weight: bold;'>Rs.{bill.BreakfastCharges:N2}</td>
                </tr>
                <tr style='background: #dbeafe;'>
                    <td style='padding: 12px; border: 1px solid #e5e7eb;'>🌤️ Lunch</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; color: #6b7280;'>{bill.LunchCount} × Rs.{bill.LunchRate}</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; font-weight: bold;'>Rs.{bill.LunchCharges:N2}</td>
                </tr>
                <tr style='background: #e9d5ff;'>
                    <td style='padding: 12px; border: 1px solid #e5e7eb;'>🌙 Dinner</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; color: #6b7280;'>{bill.DinnerCount} × Rs.{bill.DinnerRate}</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; font-weight: bold;'>Rs.{bill.DinnerCharges:N2}</td>
                </tr>
                <tr style='background: #ecfeff;'>
                    <td style='padding: 12px; border: 1px solid #e5e7eb;'>💧 Water</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; color: #6b7280;'>{totalMeals} meals</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; font-weight: bold; color: #10b981;'>FREE</td>
                </tr>
                <tr style='background: #fff7ed;'>
                    <td style='padding: 12px; border: 1px solid #e5e7eb;'>☕ Tea/Coffee</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; color: #6b7280;'>{totalMeals} meals × Rs.100</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; font-weight: bold;'>Rs.{bill.TeaCharges:N2}</td>
                </tr>
                <tr style='background: #10b981; color: white;'>
                    <td colspan='2' style='padding: 12px; border: 1px solid #059669; font-weight: bold;'>Grand Total</td>
                    <td style='padding: 12px; border: 1px solid #059669; text-align: right; font-weight: bold; font-size: 18px;'>Rs.{bill.GrandTotal:N2}</td>
                </tr>
            </table>

            <!-- Payment Summary -->
            <h3 style='color: #374151; margin-top: 25px;'>💳 Payment Summary</h3>
            <table style='width: 100%; border-collapse: collapse; margin: 10px 0;'>
                <tr style='background: #f9fafb;'>
                    <td style='padding: 12px; border: 1px solid #e5e7eb;'>Total Charges</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; font-weight: bold;'>Rs.{bill.GrandTotal:N2}</td>
                </tr>
                <tr>
                    <td style='padding: 12px; border: 1px solid #e5e7eb;'>Amount Paid</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; font-weight: bold; color: #10b981;'>Rs.{bill.AmountPaid:N2}</td>
                </tr>
                <tr style='background: {statusBg};'>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; font-weight: bold;'>{statusText}</td>
                    <td style='padding: 12px; border: 1px solid #e5e7eb; text-align: right; font-weight: bold; font-size: 18px; color: {statusColor};'>Rs.{bill.Balance:N2}</td>
                </tr>
            </table>

            {(bill.Balance > 0 ? @"
            <div style='margin: 25px 0; padding: 15px; background: #fef3c7; border-radius: 8px; border-left: 4px solid #f59e0b;'>
                <p style='margin: 0; color: #92400e;'><strong>⚠️ Payment Reminder:</strong></p>
                <p style='margin: 5px 0 0 0; color: #92400e;'>Please clear your dues at your earliest convenience to avoid any inconvenience.</p>
            </div>
            " : @"
            <div style='margin: 25px 0; padding: 15px; background: #d1fae5; border-radius: 8px; border-left: 4px solid #10b981;'>
                <p style='margin: 0; color: #065f46;'><strong>✅ Thank you!</strong></p>
                <p style='margin: 5px 0 0 0; color: #065f46;'>Your account is fully paid for this month.</p>
            </div>
            ")}

            <p>If you have any questions about this bill, please contact the mess administrator.</p>
            <p>Best regards,<br>Mess Management Team</p>
        ");

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendMonthlyBillWithPdfAsync(string toEmail, string memberName, MonthlyBillEmailModel bill, byte[] pdfAttachment, string fileName)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:Username"] ?? "";
            var smtpPass = _configuration["Email:Password"] ?? "";
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
            var fromName = _configuration["Email:FromName"] ?? "Mess Management System";

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                _logger.LogWarning("Email not configured. Skipping email to {Email}", toEmail);
                return false;
            }

            var subject = $"Monthly Bill - {bill.MonthName} {bill.Year} | Mess Management";
            
            var statusColor = bill.Balance > 0 ? "#dc2626" : "#10b981";
            var statusBg = bill.Balance > 0 ? "#fef2f2" : "#d1fae5";
            var statusText = bill.Balance > 0 ? "Balance Due" : "Fully Paid";
            var totalMeals = bill.BreakfastCount + bill.LunchCount + bill.DinnerCount;
            
            var body = GetEmailTemplate($"Monthly Bill - {bill.MonthName} {bill.Year}", $@"
                <p>Dear <strong>{memberName}</strong>,</p>
                <p>Please find attached your monthly bill for <strong>{bill.MonthName} {bill.Year}</strong>.</p>
                
                <!-- Quick Summary -->
                <div style='margin: 20px 0; padding: 20px; background: linear-gradient(135deg, #f0fdf4, #dcfce7); border-radius: 12px; border: 1px solid #86efac;'>
                    <h3 style='margin: 0 0 15px 0; color: #166534;'>📊 Quick Summary</h3>
                    <table style='width: 100%;'>
                        <tr>
                            <td style='padding: 5px 0; color: #374151;'>Total Meals:</td>
                            <td style='padding: 5px 0; text-align: right; font-weight: bold;'>{totalMeals} meals</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px 0; color: #374151;'>Grand Total:</td>
                            <td style='padding: 5px 0; text-align: right; font-weight: bold; color: #059669;'>Rs.{bill.GrandTotal:N2}</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px 0; color: #374151;'>Amount Paid:</td>
                            <td style='padding: 5px 0; text-align: right; font-weight: bold; color: #10b981;'>Rs.{bill.AmountPaid:N2}</td>
                        </tr>
                        <tr style='border-top: 1px solid #86efac;'>
                            <td style='padding: 10px 0 5px 0; color: #374151; font-weight: bold;'>{statusText}:</td>
                            <td style='padding: 10px 0 5px 0; text-align: right; font-weight: bold; font-size: 20px; color: {statusColor};'>Rs.{bill.Balance:N2}</td>
                        </tr>
                    </table>
                </div>

                {(bill.Balance > 0 ? @"
                <div style='margin: 20px 0; padding: 15px; background: #fef3c7; border-radius: 8px; border-left: 4px solid #f59e0b;'>
                    <p style='margin: 0; color: #92400e;'><strong>⚠️ Payment Reminder:</strong></p>
                    <p style='margin: 5px 0 0 0; color: #92400e;'>Please clear your dues at your earliest convenience.</p>
                </div>
                " : @"
                <div style='margin: 20px 0; padding: 15px; background: #d1fae5; border-radius: 8px; border-left: 4px solid #10b981;'>
                    <p style='margin: 0; color: #065f46;'><strong>✅ Thank you!</strong> Your account is fully paid for this month.</p>
                </div>
                ")}

                <p style='color: #6b7280; font-size: 14px;'>📎 <em>Detailed bill is attached as PDF for your records.</em></p>
                <p>Best regards,<br>Mess Management Team</p>
            ");

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            // Attach PDF
            using var pdfStream = new MemoryStream(pdfAttachment);
            var attachment = new Attachment(pdfStream, fileName, "application/pdf");
            mailMessage.Attachments.Add(attachment);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Bill email with PDF sent successfully to {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send bill email with PDF to {Email}", toEmail);
            return false;
        }
    }

    private string GetEmailTemplate(string title, string content)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #10b981 0%, #14b8a6 100%); padding: 30px; border-radius: 12px 12px 0 0; text-align: center;'>
        <h1 style='color: white; margin: 0; font-size: 24px;'>🍽️ Mess Management</h1>
    </div>
    <div style='background: white; padding: 30px; border: 1px solid #e5e7eb; border-top: none; border-radius: 0 0 12px 12px;'>
        <h2 style='color: #10b981; margin-top: 0;'>{title}</h2>
        {content}
        <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;'>
        <p style='color: #9ca3af; font-size: 12px; text-align: center;'>
            This is an automated message from Mess Management System.<br>
            Please do not reply to this email.
        </p>
    </div>
</body>
</html>";
    }
}

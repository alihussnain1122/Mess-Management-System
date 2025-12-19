using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using DotNetEnv;

// Load environment variables from .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add file logging for production
if (!builder.Environment.IsDevelopment())
{
    var logPath = Path.Combine(builder.Environment.ContentRootPath, "logs");
    if (!Directory.Exists(logPath))
    {
        Directory.CreateDirectory(logPath);
    }
}

// Add services to the container
builder.Services.AddRazorPages();

// Get secrets from environment variables
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
var stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
var stripePublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
var stripeWebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
var emailSmtpHost = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST");
var emailSmtpPort = Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT");
var emailUsername = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
var emailFrom = Environment.GetEnvironmentVariable("EMAIL_FROM");
var emailFromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME");

// Configure Entity Framework with SQL Server using env connection string
builder.Services.AddDbContext<MessDbContext>(options =>
    options.UseSqlServer(connectionString)
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

// Register Services (Repository-Service Pattern)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IWaterTeaService, WaterTeaService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();

// Configure Stripe Settings using environment variables
builder.Services.Configure<StripeSettings>(options =>
{
    options.SecretKey = stripeSecretKey;
    options.PublishableKey = stripePublishableKey;
    options.WebhookSecret = stripeWebhookSecret;
});

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Optionally, you can register email settings as a singleton or configuration object if needed
// Example:
// builder.Services.Configure<EmailSettings>(options =>
// {
//     options.SmtpHost = emailSmtpHost;
//     options.SmtpPort = emailSmtpPort;
//     options.Username = emailUsername;
//     options.Password = emailPassword;
//     options.FromEmail = emailFrom;
//     options.FromName = emailFromName;
// });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MessDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception occurred. Path: {Path}", context.Request.Path);
        throw;
    }
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

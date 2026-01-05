using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;

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

// ============================================
// MEMORY CACHING (Performance Optimization)
// ============================================
builder.Services.AddMemoryCache();

// Configure Entity Framework with SQL Server using appsettings.json
builder.Services.AddDbContext<MessDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => {
            sqlOptions.CommandTimeout(120); // Increase timeout to 120 seconds
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null); // Retry on transient failures
        }));

// ============================================
// DEPENDENCY INJECTION - ALL THREE LIFETIMES
// ============================================

// SINGLETON: One instance for entire application lifetime
// Use for: Application settings, Caching, Shared state
builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();

// TRANSIENT: New instance every time it's requested
// Use for: Lightweight, stateless services
builder.Services.AddTransient<IGuidGeneratorService, GuidGeneratorService>();

// SCOPED: One instance per HTTP request (most common for web apps)
// Use for: Database contexts, Services that need request-level state
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

// Configure Stripe Settings from appsettings.json
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// ============================================
// COOKIE AUTHENTICATION (with Security Hardening)
// ============================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        
        // ============================================
        // SECURITY HARDENING - Cookie Settings
        // ============================================
        options.Cookie.HttpOnly = true;           // Prevents JavaScript access (XSS protection)
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Works with both HTTP and HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;  // Allows login redirects to work
        options.Cookie.Name = ".DineSync.Auth";   // Custom cookie name
        options.Cookie.IsEssential = true;        // Required for authentication
    });

// ============================================
// COOKIE POLICY (GDPR Compliance)
// ============================================
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false; // Set to true for GDPR consent
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.SameAsRequest;
});

// ============================================
// AUTHORIZATION POLICIES
// ============================================
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

// ============================================
// MIDDLEWARE PIPELINE (Order is CRITICAL!)
// ============================================

// 1. Exception handling must be FIRST
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detailed errors in development
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // HTTP Strict Transport Security
    
    // 2. HTTPS Redirection - Only in production (requires HTTPS to be configured)
    app.UseHttpsRedirection();
}

// 3. Global exception logging middleware
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
        throw; // Re-throw to let UseExceptionHandler handle it
    }
});

// 4. Cookie Policy (GDPR compliance)
app.UseCookiePolicy();

// 5. Static files (before routing for performance)
app.UseStaticFiles();

// 6. Routing
app.UseRouting();

// 7. Authentication (must be after UseRouting, before UseAuthorization)
app.UseAuthentication();

// 8. Authorization (must be after UseAuthentication)
app.UseAuthorization();

// 9. Map endpoints
app.MapRazorPages();

app.Run();

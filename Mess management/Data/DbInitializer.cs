using MessManagement.Data;
using MessManagement.Helpers;
using MessManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Data;

public static class DbInitializer
{
    public static void Initialize(MessDbContext context)
    {
        context.Database.EnsureCreated();
        
        // Add MenuDate column if it doesn't exist
        AddMenuDateColumnIfNotExists(context);
        
        SeedAdminUser(context);
    }

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MessDbContext>();
        
        // Don't use MigrateAsync as it conflicts with existing tables
        await context.Database.EnsureCreatedAsync();
        
        // Add MenuDate column if it doesn't exist
        await AddMenuDateColumnIfNotExistsAsync(context);
        
        await SeedAdminUserAsync(context);
    }
    
    private static void AddMenuDateColumnIfNotExists(MessDbContext context)
    {
        try
        {
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WeeklyMenus') AND name = 'MenuDate')
                BEGIN
                    ALTER TABLE [WeeklyMenus] ADD [MenuDate] datetime2 NULL;
                END");
        }
        catch { /* Column might already exist */ }
    }
    
    private static async Task AddMenuDateColumnIfNotExistsAsync(MessDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WeeklyMenus') AND name = 'MenuDate')
                BEGIN
                    ALTER TABLE [WeeklyMenus] ADD [MenuDate] datetime2 NULL;
                END");
        }
        catch { /* Column might already exist */ }
    }

    private static void SeedAdminUser(MessDbContext context)
    {
        if (context.Users.Any(u => u.Username == "admin"))
            return;

        var (hash, salt) = PasswordHelper.HashPassword("Admin@123");
        
        var adminUser = new User
        {
            Username = "admin",
            Email = "alihussnaintech@gmail.com",
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
    }

    private static async Task SeedAdminUserAsync(MessDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Username == "admin"))
            return;

        var (hash, salt) = PasswordHelper.HashPassword("Admin@123");
        
        var adminUser = new User
        {
            Username = "admin",
            Email = "alihussnaintech@gmail.com",
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
    }
}

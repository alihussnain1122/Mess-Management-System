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
        
        SeedAdminUser(context);
    }

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MessDbContext>();
        
        await context.Database.MigrateAsync();
        
        await SeedAdminUserAsync(context);
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

using Microsoft.EntityFrameworkCore;
using MessManagement.Models;

namespace MessManagement.Data;

public class MessDbContext : DbContext
{
    public MessDbContext(DbContextOptions<MessDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<WeeklyMenu> WeeklyMenus { get; set; } = null!;
    public DbSet<Attendance> Attendances { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<WaterTea> WaterTeaRecords { get; set; } = null!;
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(50);
            entity.Property(u => u.Role).HasConversion<string>();
        });

        // Member Configuration
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasIndex(m => m.RoomNumber);
            entity.HasOne(m => m.User)
                  .WithOne(u => u.Member)
                  .HasForeignKey<Member>(m => m.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // WeeklyMenu Configuration
        modelBuilder.Entity<WeeklyMenu>(entity =>
        {
            entity.HasIndex(w => new { w.DayOfWeek, w.MealType });
            entity.Property(w => w.DayOfWeek).HasConversion<string>();
            entity.Property(w => w.MealType).HasConversion<string>();
        });

        // Attendance Configuration
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasIndex(a => new { a.MemberId, a.Date }).IsUnique();
            // BreakfastPresent, LunchPresent, DinnerPresent are stored as boolean columns
            // Status is a computed property (NotMapped)
            entity.HasOne(a => a.Member)
                  .WithMany(m => m.Attendances)
                  .HasForeignKey(a => a.MemberId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(a => a.MarkedByUser)
                  .WithMany()
                  .HasForeignKey(a => a.MarkedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment Configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(p => p.MemberId);
            entity.HasIndex(p => p.Date);
            entity.Property(p => p.PaymentMode).HasConversion<string>();
            entity.Property(p => p.Status).HasConversion<string>();
            entity.HasOne(p => p.Member)
                  .WithMany(m => m.Payments)
                  .HasForeignKey(p => p.MemberId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // WaterTea Configuration
        modelBuilder.Entity<WaterTea>(entity =>
        {
            entity.HasIndex(w => new { w.MemberId, w.Date }).IsUnique();
            entity.HasOne(w => w.Member)
                  .WithMany(m => m.WaterTeaRecords)
                  .HasForeignKey(w => w.MemberId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Menu Data only - Admin user is seeded by DbInitializer
        SeedMenuData(modelBuilder);
    }

    private void SeedMenuData(ModelBuilder modelBuilder)
    {
        // Seed Weekly Menu
        var weeklyMenus = new List<WeeklyMenu>
        {
            new() { Id = 1, DayOfWeek = DayOfWeek.Monday, DishName = "Rice with Dal", Price = 50, MealType = MealType.Lunch },
            new() { Id = 2, DayOfWeek = DayOfWeek.Monday, DishName = "Roti with Sabzi", Price = 45, MealType = MealType.Dinner },
            new() { Id = 3, DayOfWeek = DayOfWeek.Tuesday, DishName = "Biryani", Price = 80, MealType = MealType.Lunch },
            new() { Id = 4, DayOfWeek = DayOfWeek.Tuesday, DishName = "Chapati with Curry", Price = 50, MealType = MealType.Dinner },
            new() { Id = 5, DayOfWeek = DayOfWeek.Wednesday, DishName = "Fried Rice", Price = 60, MealType = MealType.Lunch },
            new() { Id = 6, DayOfWeek = DayOfWeek.Wednesday, DishName = "Paratha with Curd", Price = 55, MealType = MealType.Dinner },
            new() { Id = 7, DayOfWeek = DayOfWeek.Thursday, DishName = "Paneer Curry with Rice", Price = 75, MealType = MealType.Lunch },
            new() { Id = 8, DayOfWeek = DayOfWeek.Thursday, DishName = "Khichdi", Price = 40, MealType = MealType.Dinner },
            new() { Id = 9, DayOfWeek = DayOfWeek.Friday, DishName = "Chicken Curry", Price = 100, MealType = MealType.Lunch },
            new() { Id = 10, DayOfWeek = DayOfWeek.Friday, DishName = "Egg Curry with Rice", Price = 70, MealType = MealType.Dinner },
            new() { Id = 11, DayOfWeek = DayOfWeek.Saturday, DishName = "Special Thali", Price = 120, MealType = MealType.Lunch },
            new() { Id = 12, DayOfWeek = DayOfWeek.Saturday, DishName = "Pulao", Price = 65, MealType = MealType.Dinner },
            new() { Id = 13, DayOfWeek = DayOfWeek.Sunday, DishName = "Fish Curry", Price = 110, MealType = MealType.Lunch },
            new() { Id = 14, DayOfWeek = DayOfWeek.Sunday, DishName = "Mixed Veg Rice", Price = 55, MealType = MealType.Dinner }
        };

        modelBuilder.Entity<WeeklyMenu>().HasData(weeklyMenus);
    }
}

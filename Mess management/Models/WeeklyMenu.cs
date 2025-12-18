using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessManagement.Models;

public class WeeklyMenu
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    // Specific date for the menu (allows monthly planning)
    public DateTime? MenuDate { get; set; }

    [Required]
    [StringLength(100)]
    public string DishName { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public MealType MealType { get; set; } = MealType.Lunch;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Computed property to get effective day
    [NotMapped]
    public DayOfWeek EffectiveDayOfWeek => MenuDate?.DayOfWeek ?? DayOfWeek;
}

public enum MealType
{
    Breakfast,
    Lunch,
    Dinner
}

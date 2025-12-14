using System.ComponentModel.DataAnnotations;
using MessManagement.Models;

namespace MessManagement.ViewModels;

public class WaterTeaSummary
{
    public int MemberId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalWaterCount { get; set; }
    public int TotalTeaCount { get; set; }
    public decimal TotalCost { get; set; }
    public IEnumerable<WaterTea> Records { get; set; } = new List<WaterTea>();
}

public class AddWaterTeaViewModel
{
    [Required(ErrorMessage = "Member is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a member")]
    public int MemberId { get; set; }
    
    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; } = DateTime.Today;
    
    [Range(0, 100, ErrorMessage = "Water count must be between 0 and 100")]
    public int WaterCount { get; set; }
    
    [Range(0, 100, ErrorMessage = "Tea count must be between 0 and 100")]
    public int TeaCount { get; set; }
}

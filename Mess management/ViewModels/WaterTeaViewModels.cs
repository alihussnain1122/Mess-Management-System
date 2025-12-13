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
    public int MemberId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public int WaterCount { get; set; }
    public int TeaCount { get; set; }
}

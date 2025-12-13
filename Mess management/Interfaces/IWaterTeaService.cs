using MessManagement.Models;
using MessManagement.ViewModels;

namespace MessManagement.Interfaces;

public interface IWaterTeaService
{
    Task<IEnumerable<WaterTea>> GetAllRecordsAsync();
    Task<IEnumerable<WaterTea>> GetRecordsForMemberAsync(int memberId);
    Task<WaterTea?> GetRecordByIdAsync(int id);
    Task<WaterTea?> GetRecordAsync(int memberId, DateTime date);
    Task<WaterTea> AddRecordAsync(WaterTea record);
    Task<WaterTea> UpdateRecordAsync(WaterTea record);
    Task<bool> DeleteRecordAsync(int id);
    Task<decimal> CalculateMonthlyCostAsync(int memberId, int month, int year);
    Task<WaterTeaSummary> GetMonthlySummaryAsync(int memberId, int month, int year);
}

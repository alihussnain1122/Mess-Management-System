using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Services;

public class WaterTeaService : IWaterTeaService
{
    private readonly MessDbContext _context;
    private const decimal WaterCostPerUnit = 5.00m;
    private const decimal TeaCostPerUnit = 10.00m;

    public WaterTeaService(MessDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WaterTea>> GetAllRecordsAsync()
    {
        return await _context.WaterTeaRecords
            .Include(w => w.Member)
            .OrderByDescending(w => w.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<WaterTea>> GetRecordsForMemberAsync(int memberId)
    {
        return await _context.WaterTeaRecords
            .Where(w => w.MemberId == memberId)
            .OrderByDescending(w => w.Date)
            .ToListAsync();
    }

    public async Task<WaterTea?> GetRecordByIdAsync(int id)
    {
        return await _context.WaterTeaRecords
            .Include(w => w.Member)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WaterTea?> GetRecordAsync(int memberId, DateTime date)
    {
        return await _context.WaterTeaRecords
            .FirstOrDefaultAsync(w => w.MemberId == memberId && w.Date.Date == date.Date);
    }

    public async Task<WaterTea> AddRecordAsync(WaterTea record)
    {
        record.Cost = CalculateCost(record.WaterCount, record.TeaCount);
        record.CreatedAt = DateTime.UtcNow;

        _context.WaterTeaRecords.Add(record);
        await _context.SaveChangesAsync();

        return record;
    }

    public async Task<WaterTea> UpdateRecordAsync(WaterTea record)
    {
        var existingRecord = await _context.WaterTeaRecords.FindAsync(record.Id);

        if (existingRecord == null)
            throw new ArgumentException("Record not found", nameof(record));

        existingRecord.WaterCount = record.WaterCount;
        existingRecord.TeaCount = record.TeaCount;
        existingRecord.Cost = CalculateCost(record.WaterCount, record.TeaCount);
        existingRecord.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return existingRecord;
    }

    public async Task<bool> DeleteRecordAsync(int id)
    {
        var record = await _context.WaterTeaRecords.FindAsync(id);

        if (record == null)
            return false;

        _context.WaterTeaRecords.Remove(record);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<decimal> CalculateMonthlyCostAsync(int memberId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        return await _context.WaterTeaRecords
            .Where(w => w.MemberId == memberId && w.Date >= startDate && w.Date <= endDate)
            .SumAsync(w => w.Cost);
    }

    public async Task<WaterTeaSummary> GetMonthlySummaryAsync(int memberId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var records = await _context.WaterTeaRecords
            .Where(w => w.MemberId == memberId && w.Date >= startDate && w.Date <= endDate)
            .ToListAsync();

        return new WaterTeaSummary
        {
            MemberId = memberId,
            Month = month,
            Year = year,
            TotalWaterCount = records.Sum(r => r.WaterCount),
            TotalTeaCount = records.Sum(r => r.TeaCount),
            TotalCost = records.Sum(r => r.Cost),
            Records = records
        };
    }

    private static decimal CalculateCost(int waterCount, int teaCount)
    {
        return (waterCount * WaterCostPerUnit) + (teaCount * TeaCostPerUnit);
    }
}

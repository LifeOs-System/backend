using Domain.Entities.HabitsRecords;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HabitRecordRepository : IHabitRecordRepository
{
    private readonly AppDbContext _dbContext;

    public HabitRecordRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HabitRecord?> GetHabitRecordByHabit(
        Guid habitId,
        DateOnly? date = null)
    {
        return await _dbContext.HabitRecords
            .FirstOrDefaultAsync(r => r.HabitId == habitId && r.Date == date);
    }

    public async Task<List<HabitRecord>> GetHabitRecordsLastWeek()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var startDate = today.AddDays(-7);  // 16 - 7 = 9
        var endDate = today.AddDays(-1);    // 16 - 1 = 15

        return await _dbContext.HabitRecords
            .Where(hr => hr.Date >= startDate && hr.Date <= endDate)
            .Include(hr => hr.Habit)
            .OrderBy(hr => hr.Date)
            .ToListAsync();
    }

    public async Task<List<HabitRecord>> GetHabitRecordsPreviousWeek()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var startDate = today.AddDays(-14); // 16 - 14 = 2
        var endDate = today.AddDays(-8);    // 16 - 8  = 8

        return await _dbContext.HabitRecords
            .Where(hr => hr.Date >= startDate && hr.Date <= endDate)
            .Include(hr => hr.Habit)
            .OrderBy(hr => hr.Date)
            .ToListAsync();
    }

    public async Task UpdateAsync(HabitRecord record)
    {
        _dbContext.HabitRecords.Update(record);
        await _dbContext.SaveChangesAsync();
    }
}

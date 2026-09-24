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


    public async Task<List<HabitRecord>> GetHabitRecordsByHabit(Guid habitId)
    {
        return await _dbContext.HabitRecords.Where(r => r.HabitId == habitId).ToListAsync();
    }

    public async Task UpdateAsync(HabitRecord record)
    {
        _dbContext.HabitRecords.Update(record);
        await _dbContext.SaveChangesAsync();
    }
}

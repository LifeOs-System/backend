using Domain.Entities.Habits;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HabitRepository : IHabitRepository
{
    private readonly AppDbContext _dbContext;
    public HabitRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Habit habit)
    {
        _dbContext.Habits.Add(habit);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Habit>> GetAllAsync()
    {
        return await _dbContext.Habits
            .Include(h => h.Records)
            .Where(h => h.Status != HabitStatus.Deleted)
            .OrderBy(h => h.Name)
            .ToListAsync();
    }

    public async Task<Habit?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Habits
            .Where(h => h.Status != HabitStatus.Deleted)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<List<Habit>> GetHabitsByDayOfWeekAsync(DayOfWeek dayOfWeek)
    {
        return await _dbContext.Habits
             .Include(h => h.Records)
             .Where(h => h.Status != HabitStatus.Deleted && h.Days.Contains(dayOfWeek))
             .ToListAsync();
    }
}

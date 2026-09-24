namespace Domain.Entities.Habits;

public interface IHabitRepository
{
    Task<List<Habit>> GetAllAsync();
    Task<Habit?> GetByIdAsync(Guid id);
    Task<List<Habit>> GetHabitsForTodayAsync(DateOnly todayDate, DayOfWeek currentDayOfWeek);
    Task CreateAsync(Habit habit);
}

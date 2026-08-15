namespace Domain.Entities.Habits;

public interface IHabitRepository
{
    Task<List<Habit>> GetAllAsync();
    Task<Habit?> GetByIdAsync(Guid id);
    Task CreateAsync(Habit habit);
    Task<List<Habit>> GetHabitsByDayOfWeekAsync(DayOfWeek dayOfWeek);
}

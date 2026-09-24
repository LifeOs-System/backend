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

    public async Task<List<Habit>> GetHabitsForTodayAsync(DateOnly todayDate, DayOfWeek currentDayOfWeek)
    {
        // Calcular el Lunes y el Domingo de la semana actual
        // (En C#, DayOfWeek.Lunes es 1, Domingo es 0)
        int daysToMonday = currentDayOfWeek == DayOfWeek.Sunday ? 6 : (int)currentDayOfWeek - 1;
        var startOfWeek = todayDate.AddDays(-daysToMonday);
        var endOfWeek = startOfWeek.AddDays(6);

        return await _dbContext.Habits
            .Include(h => h.Records) // Incluimos los registros para contar los completados
            .Where(h => h.Status == HabitStatus.Active && (

                // CASO 1: El hábito tiene días específicos y hoy es uno de ellos
                h.Days.Contains(currentDayOfWeek)

                ||

                // CASO 2: El hábito es SEMANAL y aún no ha completado sus ocurrencias esta semana
                (h.Frequency == HabitFrequency.Weekly &&
                 h.Records.Count(r => r.Date >= startOfWeek && r.Date <= endOfWeek && r.IsCompleted) < h.Occurrences)

            // (Opcional) CASO 3: Si también manejas hábitos mensuales, agregarías la lógica aquí
            ))
            .ToListAsync();
    }
}

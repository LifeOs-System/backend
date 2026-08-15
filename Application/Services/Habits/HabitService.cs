using Application.Services.Habits.Create;
using Application.Services.Habits.GetAll;
using Application.Services.Habits.GetToday;
using Domain.Entities.Habits;

namespace Application.Services.Habits;

public class HabitService : IHabitService
{
    private readonly IHabitRepository _habitRepository;
    public HabitService(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository;
    }

    public async Task CreateAsync(CreateHabitRequest request)
    {
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            Area = request.Area,
            Target = request.Target,
            Unit = request.Unit,
            Days = request.Days.ToList()
        };

        await _habitRepository.CreateAsync(habit);
        await Task.CompletedTask;
    }

    public async Task<List<HabitResponse>> GetAllAsync()
    {
        var habits = await _habitRepository.GetAllAsync();

        return habits.Select(habit =>
        {
            // Días transcurridos desde el inicio (incluyendo el día de inicio, siempre ≥ 1)
            var totalDays = habit.StartDate.HasValue
                ? (DateTime.UtcNow.DayOfYear - habit.StartDate.Value.DayOfYear) + 1
                : 0;

            var completedDays = habit.Records.Count(r => r.IsCompleted);

            // % de cumplimiento, acotado a 0–100 y sin división por cero
            var completionRate = totalDays > 0
                ? Math.Min(100, Math.Round((decimal)completedDays * 100 / totalDays, 1))
                : (completedDays > 0 ? 100 : 0);

            return new HabitResponse
            {
                Id = habit.Id.ToString(),
                Name = habit.Name,
                Type = habit.Type,
                Status = habit.Status,
                StartDate = habit.StartDate,
                Target = habit.Target,
                Unit = habit.Unit,
                Area = habit.Area,
                Days = habit.Days.ToList(),
                TotalDays = totalDays,
                CompletedDays = completedDays,
                CompletionRate = completionRate
            };
        }).ToList();
    }
    public async Task<List<HabitTodayResponse>> GetHabitsTodayAsync()
    {
        // Usa la MISMA fuente de "hoy" para todo
        var today = DateTime.Today;
        var todayDate = DateOnly.FromDateTime(today);

        var habits = await _habitRepository.GetHabitsByDayOfWeekAsync(today.DayOfWeek);

        var habitsResponse = new List<HabitTodayResponse>();

        foreach (var habit in habits)
        {
            var r = new HabitTodayResponse
            {
                Area = habit.Area,
                Id = habit.Id.ToString(),
                Name = habit.Name,
                Type = habit.Type,
                Target = habit.Target,
                Unit = habit.Unit
            };

            var recordToday = habit.Records.FirstOrDefault(hr => hr.Date == todayDate);

            // Sin "!" : si no hay registro, devuelves valores por defecto
            r.IsCompleted = recordToday?.IsCompleted ?? false;

            if (habit.Type != HabitType.Binary)
                r.Value = recordToday?.Value ?? 0;

            habitsResponse.Add(r);
        }

        return habitsResponse;
    }
}

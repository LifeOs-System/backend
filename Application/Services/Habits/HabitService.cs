using Application.Services.Habits.Create;
using Application.Services.Habits.GetAll;
using Application.Services.Habits.GetToday;
using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;

namespace Application.Services.Habits;

public class HabitService : IHabitService
{
    private readonly IHabitRepository _habitRepository;
    private readonly IHabitRecordRepository _habitRecordRepository;
    public HabitService(IHabitRepository habitRepository, IHabitRecordRepository habitRecordRepository)
    {
        _habitRepository = habitRepository;
        _habitRecordRepository = habitRecordRepository;
    }

    public async Task CreateAsync(CreateHabitRequest request)
    {
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            Area = request.Area,
            Frequency = request.Frequency.Value,
            Occurrences = request.Occurrences,
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
        var responses = new List<HabitResponse>();

        // Usamos foreach para que las consultas se ejecuten una por una (secuencialmente)
        foreach (var habit in habits)
        {
            var habitRecords = await _habitRecordRepository.GetHabitRecordsByHabit(habit.Id);
            var totalDays = habitRecords.Count;
            var completedDays = habitRecords.Count(r => r.IsCompleted);

            var completionRate = totalDays > 0
                ? Math.Min(100m, Math.Round((decimal)completedDays * 100 / totalDays, 1))
                : 0m;

            responses.Add(new HabitResponse
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
                CompletionRate = completionRate,
                Frequency = habit.Frequency,
                Occurrences = habit.Occurrences
            });
        }

        return responses;
    }

    public async Task<List<HabitTodayResponse>> GetHabitsTodayAsync()
    {
        var today = DateTime.Today;
        var todayDate = DateOnly.FromDateTime(today);
        var currentDayOfWeek = today.DayOfWeek;

        var habits = await _habitRepository.GetHabitsForTodayAsync(todayDate, currentDayOfWeek);

        var habitsResponse = new List<HabitTodayResponse>();

        foreach (var habit in habits)
        {
            var recordToday = habit.Records.FirstOrDefault(hr => hr.Date == todayDate);

            var response = new HabitTodayResponse
            {
                Id = habit.Id.ToString(),
                Name = habit.Name,
                Type = habit.Type,
                Area = habit.Area,
                Target = habit.Target,
                Unit = habit.Unit,
                IsCompleted = recordToday?.IsCompleted ?? false,
                Value = habit.Type != HabitType.Binary ? (recordToday?.Value ?? 0m) : null,
                Frequency = habit.Frequency
            };

            // ✅ LÓGICA CONDICIONAL: Solo calcular si tiene Frecuencia y Ocurrencias definidas
            if (habit.Frequency != null && habit.Occurrences != null)
            {
                response.Occurrences = habit.Occurrences;

                // Calcular el rango de fechas exacto para el conteo
                DateOnly startDate;
                DateOnly endDate;

                if (habit.Frequency == HabitFrequency.Weekly)
                {
                    // Rango: Lunes a Domingo de la semana actual
                    int daysToMonday = currentDayOfWeek == DayOfWeek.Sunday ? 6 : (int)currentDayOfWeek - 1;
                    startDate = todayDate.AddDays(-daysToMonday);
                    endDate = startDate.AddDays(6);
                }
                else // Monthly
                {
                    // Rango: Día 1 al último día del mes actual
                    startDate = new DateOnly(today.Year, today.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }

                // Contar SOLO los registros completados dentro de ese rango
                response.CompletedOccurrences = habit.Records.Count(r =>
                    r.Date >= startDate && r.Date <= endDate && r.IsCompleted);
            }
            else
            {
                // Si es un hábito de días específicos, estos campos son irrelevantes (null)
                response.Occurrences = null;
                response.CompletedOccurrences = null;
            }

            habitsResponse.Add(response);
        }

        return habitsResponse;
    }
}

using Application.Services.HabitRecords.Create;
using Application.Services.HabitRecords.GetLastWeek;
using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;
using Infrastructure.Repositories;

namespace Application.Services.HabitRecords;

public class HabitRecordService : IHabitRecordService
{
    private readonly IHabitRepository _habitRepository;
    private readonly IHabitRecordRepository _habitRecordRepository;

    public HabitRecordService(
        IHabitRepository habitRepository,
        IHabitRecordRepository habitRecordRepository)
    {
        _habitRepository = habitRepository;
        _habitRecordRepository = habitRecordRepository;
    }
    public async Task<string?> CreateAsync(CreateHabitRecordRequest req)
    {
        var habit = await _habitRepository.GetByIdAsync(Guid.Parse(req.HabitId));

        if (habit is null)
            return "El habito no existe";

        var habitRecordToday = await _habitRecordRepository.GetHabitRecordByHabit(Guid.Parse(req.HabitId), DateOnly.FromDateTime(DateTime.Today));
        var habitType = habit.Type;

        if (habitType == HabitType.Binary)
        {
            habitRecordToday!.IsCompleted = req.IsCompleted!.Value;
        }
        else
        {
            habitRecordToday!.Value = req.Value;
            habitRecordToday.IsCompleted = habitRecordToday.Value >= habitRecordToday.Target;
        }
        await _habitRecordRepository.UpdateAsync(habitRecordToday);

        return null;
    }

    public async Task<GetLastWeekResponse> GetLastWeekSummaryAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        // La semana mostrada: NO incluye hoy
        var endDate = today.AddDays(-1);
        var startDate = endDate.AddDays(-6); // 7 días exactos

        var currentRecords = await _habitRecordRepository.GetHabitRecordsLastWeek();
        var previousRecords = await _habitRecordRepository.GetHabitRecordsPreviousWeek();

        // ⭐ Incluir los días programados del hábito para poder contar
        var habits = currentRecords
            .Select(r => r.Habit)
            .DistinctBy(h => h.Id)
            .ToList();

        var response = new GetLastWeekResponse
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalHabits = habits.Count,
        };

        // Columnas S 9 | D 10 | L 11 | M 12 | M 13 | J 14 | V 15
        for (var i = 0; i < 7; i++)
        {
            var date = startDate.AddDays(i);
            response.Days.Add(new WeekDayHeader
            {
                Date = date,
                DayLetter = GetLasWeekHelpers.DayLetters[(int)date.DayOfWeek],
                DayNumber = date.Day,
            });
        }

        // Filas de la semana actual
        response.Habits = habits
            .Select(h => GetLasWeekHelpers.BuildHabitWeek(h, currentRecords, startDate))
            .ToList();

        response.OverallCompletionPercentage =
            GetLasWeekHelpers.Average(response.Habits.Select(h => h.WeekCompletionPercentage));

        var ordered = response.Habits
            .OrderByDescending(h => h.WeekCompletionPercentage)
            .ToList();

        response.StarHabit = GetLasWeekHelpers.ToHighlight(ordered.FirstOrDefault());
        response.AtRiskHabit = GetLasWeekHelpers.ToHighlight(ordered.LastOrDefault());
        response.WeakestDay = GetLasWeekHelpers.BuildWeakestDay(response.Habits);

        // Semana previa
        var previousStart = startDate.AddDays(-7);
        response.WeekComparison = GetLasWeekHelpers.BuildWeekComparison(
            habits, currentRecords, previousRecords, startDate, previousStart);

        return response;
    }

}

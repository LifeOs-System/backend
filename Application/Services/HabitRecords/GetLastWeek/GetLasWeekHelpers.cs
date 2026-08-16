using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;
using Infrastructure.Repositories;


namespace Application.Services.HabitRecords.GetLastWeek;

public static class GetLasWeekHelpers
{
    public static HabitWeekSummary BuildHabitWeek(
        Habit habit, List<HabitRecord> records, DateOnly startDate)
    {
        // ⭐ Calcular cuántos días de la semana este hábito está programado
        var scheduledDayCount = CountScheduledDaysInWeek(habit, startDate);

        var summary = new HabitWeekSummary
        {
            HabitId = habit.Id,
            Name = habit.Name,
            Type = habit.Type,
            Unit = habit.Unit,
            DailyTarget = habit.Target,
            TotalDays = scheduledDayCount,  // ⭐ antes era 7 fijo
        };

        for (var i = 0; i < 7; i++)
        {
            var date = startDate.AddDays(i);
            var record = records.FirstOrDefault(r => r.HabitId == habit.Id && r.Date == date);

            summary.Days.Add(new HabitDaySummary
            {
                Date = date,
                HasRecord = record is not null,
                IsCompleted = record?.IsCompleted,
                Value = record?.Value,
                MetDailyTarget = record is not null && MeetsDailyTarget(habit, record),
            });
        }

        // ⭐ CompletedDays: solo cuenta los días donde el hábito estaba programado
        summary.CompletedDays = summary.Days.Count(d =>
            IsScheduledDay(habit, d.Date) && d.MetDailyTarget);

        // ⭐ Sumar solo los valores de los días donde había registro
        summary.WeekTotalValue = summary.Days.Sum(d => d.Value ?? 0);

        // ⭐ Target semanal = target diario × días programados (no × 7)
        summary.WeekTotalTarget = habit.Type == HabitType.Binary
            ? null
            : (habit.Target ?? 0) * scheduledDayCount;

        summary.WeekCompletionPercentage = habit.Type == HabitType.Binary
            ? Percent(summary.CompletedDays, scheduledDayCount)
            : Percent(summary.WeekTotalValue ?? 0, summary.WeekTotalTarget ?? 0);

        return summary;
    }

    // ⭐ Nuevo helper: cuántos días de la semana de 7 días está programado el hábito
    public static int CountScheduledDaysInWeek(Habit habit, DateOnly startDate)
    {
        return habit.Days.Distinct().Count();
    }

    // ⭐ Nuevo helper: verifica si un día específico está programado
    public static bool IsScheduledDay(Habit habit, DateOnly date)
    {
        return habit.Days.Contains(date.DayOfWeek);
    }

    // ──────────────────────────────────────────────────────────────────────

    public static bool MeetsDailyTarget(Habit habit, HabitRecord record) =>
        habit.Type == HabitType.Binary
            ? record.IsCompleted
            : record.Value is not null && record.Value >= (habit.Target ?? 0);

    public static HabitSummaryHighlight? ToHighlight(HabitWeekSummary? habit) =>
        habit is null
            ? null
            : new HabitSummaryHighlight
            {
                HabitId = habit.HabitId,
                HabitName = habit.Name,
                CompletedDays = habit.CompletedDays,
                TotalDays = habit.TotalDays,
            };

    public static WeakestDaySummary? BuildWeakestDay(List<HabitWeekSummary> habits)
    {
        if (habits.Count == 0) return null;

        return habits[0].Days
            .Select(day =>
            {
                // ⭐ Solo considerar hábitos programados para este día
                var scheduledHabits = habits
                    .Where(h => h.Days.Any(d => d.Date == day.Date))
                    .ToList();

                if (scheduledHabits.Count == 0)
                    return null;

                var met = scheduledHabits.Count(h =>
                    h.Days.First(d => d.Date == day.Date).MetDailyTarget);

                return new WeakestDaySummary
                {
                    Date = day.Date,
                    DayName = DayNames[(int)day.Date.DayOfWeek],
                    CompletionPercentage = Percent(met, scheduledHabits.Count),
                };
            })
            .Where(d => d is not null)
            .OrderBy(d => d!.CompletionPercentage)
            .FirstOrDefault();
    }

    public static WeekComparisonSummary BuildWeekComparison(
        List<Habit> habits,
        List<HabitRecord> currentRecords,
        List<HabitRecord> previousRecords,
        DateOnly currentStart,
        DateOnly previousStart)
    {
        var currentPct = Average(habits
            .Select(h => BuildHabitWeek(h, currentRecords, currentStart))
            .Select(h => h.WeekCompletionPercentage));

        var previousPct = Average(habits
            .Select(h => BuildHabitWeek(h, previousRecords, previousStart))
            .Select(h => h.WeekCompletionPercentage));

        return new WeekComparisonSummary
        {
            CurrentWeekPercentage = currentPct,
            PreviousWeekPercentage = previousPct,
            PercentageDifference = currentPct - previousPct,
        };
    }

    public static readonly string[] DayLetters = { "D", "L", "M", "M", "J", "V", "S" };
    public static readonly string[] DayNames =
        { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };

    public static decimal Percent(decimal value, decimal total) =>
        total == 0 ? 0 : Math.Round(value / total * 100);

    public static decimal Average(IEnumerable<decimal> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? 0 : Math.Round(list.Average(), 0);
    }
}

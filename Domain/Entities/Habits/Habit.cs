using Domain.Entities.HabitsRecords;

namespace Domain.Entities.Habits;

public class Habit
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public HabitType Type { get; set; }
    public HabitStatus Status { get; set; } = HabitStatus.Active;
    public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    // Para time / quantity
    public decimal? Target { get; set; }

    // "min", "g", "km", etc.
    public string? Unit { get; set; }

    // Relaciones
    public required Area Area { get; set; }
    public List<DayOfWeek> Days { get; set; } = new List<DayOfWeek>();
    public List<HabitRecord> Records { get; set; } = new List<HabitRecord>();
}

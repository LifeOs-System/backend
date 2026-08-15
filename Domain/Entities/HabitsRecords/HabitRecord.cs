using Domain.Entities.Habits;


namespace Domain.Entities.HabitsRecords;

public class HabitRecord
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public decimal? Value { get; set; }
    public decimal? Target { get; set; }
    public bool IsCompleted { get; set; }
    public Habit Habit { get; set; } = null!;
}
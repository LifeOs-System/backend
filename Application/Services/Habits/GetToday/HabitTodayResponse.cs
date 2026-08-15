using Domain.Entities.Habits;


namespace Application.Services.Habits.GetToday;

public class HabitTodayResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public HabitType Type { get; set; }
    public decimal? Target { get; set; }
    public decimal? Value { get; set; }
    public string? Unit { get; set; }
    public bool? IsCompleted { get; set;  }
    public required Area Area { get; set; }
}

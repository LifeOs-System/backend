using Domain.Entities.Habits;

namespace Application.Services.Habits.GetAll;

public class HabitResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public HabitType Type { get; set; }
    public HabitStatus Status { get; set; } 
    public DateOnly? StartDate { get; set; }
    public decimal? Target { get; set; }
    public string? Unit { get; set; }
    public required Area Area { get; set; }
    public List<DayOfWeek> Days { get; set; } = new List<DayOfWeek>();
    public required int TotalDays { get; set; }
    public required int CompletedDays { get; set; }
    public required decimal CompletionRate { get; set; }
}

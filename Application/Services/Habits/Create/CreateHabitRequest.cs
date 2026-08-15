using Domain.Entities.Habits;

namespace Application.Services.Habits.Create;

public class CreateHabitRequest
{
    public required string Name { get; set; }
    public HabitType Type { get; set; }
    // Para time / quantity
    public decimal? Target { get; set; }

    // "min", "g", "km", etc.
    public string? Unit { get; set; }

    // Relaciones
    public required Area Area { get; set; }
    public ICollection<DayOfWeek> Days { get; set; } = [];
}

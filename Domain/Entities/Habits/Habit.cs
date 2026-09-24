using Domain.Entities.HabitsRecords;

namespace Domain.Entities.Habits;

public class Habit
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public HabitType Type { get; set; }

    public HabitStatus Status { get; set; } = HabitStatus.Active;

    public DateOnly? StartDate { get; set; } =
        DateOnly.FromDateTime(DateTime.Today);

    // Para time / quantity
    public decimal? Target { get; set; }

    // "min", "g", "km", etc.
    public string? Unit { get; set; }

    // Tipo de frecuencia
    public HabitFrequency? Frequency { get; set; } = null;

    // Cantidad de veces que debe realizarse
    // Ej: 3 veces por semana / 5 veces por mes
    public int? Occurrences { get; set; }

    // Para hábitos realizados en días específicos
    // Ej: lunes, miércoles y viernes
    public List<DayOfWeek> Days { get; set; } = [];

    // Relaciones
    public required Area Area { get; set; }

    public List<HabitRecord> Records { get; set; } = [];
}
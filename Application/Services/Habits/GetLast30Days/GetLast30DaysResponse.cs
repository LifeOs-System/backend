using Domain.Entities.Habits;

namespace Application.Services.Habits.GetLast30DaysAsync;

public class GetLast30DaysResponse
{
    public List<Last30DaysHabitResponse> Habits { get; set; } = new List<Last30DaysHabitResponse>();
}

public class Last30DaysHabitResponse
{
    public required string Name { get; set; }
    public HabitType Type { get; set; }
    public HabitStatus Status { get; set; }
    public DateOnly? StartDate { get; set; }
    public decimal? Target { get; set; }
    public string? Unit { get; set; }
    public required Area Area { get; set; }
    public List<DayOfWeek> Days { get; set; } = new List<DayOfWeek>();
    public HabitFrequency? Frequency { get; set; }
    public int? Occurrences { get; set; }
    public List<Last30DaysHabitRecordResponse> Records { get; set; } = new List<Last30DaysHabitRecordResponse>();
}

public class Last30DaysHabitRecordResponse
{
    public DateOnly Date { get; set; }
    public decimal? Value { get; set; }
    public decimal? Target { get; set; }
    public bool IsCompleted { get; set; }
}


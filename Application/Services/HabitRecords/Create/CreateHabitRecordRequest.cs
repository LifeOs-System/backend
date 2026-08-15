namespace Application.Services.HabitRecords.Create;

public class CreateHabitRecordRequest
{
    public required string HabitId { get; set; }
    public decimal? Value { get; set; }
    public bool? IsCompleted { get; set; }
}

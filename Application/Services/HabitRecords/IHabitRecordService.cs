using Application.Services.HabitRecords.Create;


namespace Application.Services.HabitRecords;

public interface IHabitRecordService
{
    Task<string?> CreateAsync(CreateHabitRecordRequest req);
}

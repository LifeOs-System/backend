using Application.Services.HabitRecords.Create;
using Application.Services.HabitRecords.GetLastWeek;


namespace Application.Services.HabitRecords;

public interface IHabitRecordService
{
    Task<string?> CreateAsync(CreateHabitRecordRequest req);
    Task<GetLastWeekResponse> GetLastWeekSummaryAsync();
}

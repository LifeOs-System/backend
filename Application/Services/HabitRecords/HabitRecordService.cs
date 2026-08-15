using Application.Services.HabitRecords.Create;
using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;

namespace Application.Services.HabitRecords;

public class HabitRecordService : IHabitRecordService
{
    private readonly IHabitRepository _habitRepository;
    private readonly IHabitRecordRepository _habitRecordRepository;

    public HabitRecordService(
        IHabitRepository habitRepository,
        IHabitRecordRepository habitRecordRepository)
    {
        _habitRepository = habitRepository;
        _habitRecordRepository = habitRecordRepository;
    }
    public async Task<string?> CreateAsync(CreateHabitRecordRequest req)
    {
        var habit = await _habitRepository.GetByIdAsync(Guid.Parse(req.HabitId));

        if (habit is null)
            return "El habito no existe";

        var habitRecordToday = await _habitRecordRepository.GetHabitRecordByHabit(Guid.Parse(req.HabitId), DateOnly.FromDateTime(DateTime.Today));
        var habitType = habit.Type;

        if (habitType == HabitType.Binary)
        {
            habitRecordToday!.IsCompleted = req.IsCompleted!.Value;
        }
        else
        {
            habitRecordToday!.Value = req.Value;
            habitRecordToday.IsCompleted = habitRecordToday.Value >= habitRecordToday.Target;
        }
        await _habitRecordRepository.UpdateAsync(habitRecordToday);

        return null;
    }
}

namespace Domain.Entities.HabitsRecords;

public interface IHabitRecordRepository
{
    Task<HabitRecord?> GetHabitRecordByHabit(Guid habitId , DateOnly? date = null);
    Task UpdateAsync(HabitRecord record);
    Task<List<HabitRecord>> GetHabitRecordsByHabit(Guid habitId);
}

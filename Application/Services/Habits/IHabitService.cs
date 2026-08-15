using Application.Services.Habits.Create;
using Application.Services.Habits.GetAll;
using Application.Services.Habits.GetToday;

namespace Application.Services.Habits;

public interface IHabitService
{
    public Task CreateAsync(CreateHabitRequest request);
    public Task<List<HabitResponse>> GetAllAsync();
    public Task<List<HabitTodayResponse>> GetHabitsTodayAsync();
}

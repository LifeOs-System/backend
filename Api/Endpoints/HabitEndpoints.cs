using Application.Services.Habits;
using Application.Services.Habits.Create;

namespace Api.Endpoints;

public static class HabitEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/habits", async (
            CreateHabitRequest request,
            IHabitService habitService) =>
        {
            await habitService.CreateAsync(request);

            return Results.Ok();
        });

        app.MapGet("/api/habits", async (
            IHabitService habitService) =>
        {
            var habits = await habitService.GetAllAsync();
            return Results.Ok(habits);
        });

        app.MapGet("/api/habits/today", async (
            IHabitService habitService) =>
        {
            var habits = await habitService.GetHabitsTodayAsync();
            return Results.Ok(habits);
        });
    }
}
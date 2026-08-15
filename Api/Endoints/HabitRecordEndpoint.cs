using Application.Services.HabitRecords;
using Application.Services.HabitRecords.Create;


namespace Api.Endoints;

public static class HabitRecordEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/habit-record", async (
            CreateHabitRecordRequest request,
            IHabitRecordService habitRecordService) =>
        {
            var result = await habitRecordService.CreateAsync(request);

            if(result is not null)
                return Results.BadRequest(result);

            return Results.Ok();
        });
    }
}

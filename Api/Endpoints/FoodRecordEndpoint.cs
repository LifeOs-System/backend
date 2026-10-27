using Application.Services.Foods;
using Application.Services.Foods.Create;


namespace Api.Endpoints;

public static class FoodRecordEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/food-records", async (
            CreateFoodRecordRequest request,
            IFoodRecordService foodRecordService) =>
        {
            await foodRecordService.CreateAsync(request);

            return Results.Ok();
        });
    }
}

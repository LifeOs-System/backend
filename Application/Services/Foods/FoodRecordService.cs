using Application.Services.Foods.Create;
using Domain.Entities.Foods;


namespace Application.Services.Foods;

public class FoodRecordService : IFoodRecordService
{
    private readonly IFoodRecordRepository _foodRecordRepository;

    public FoodRecordService(
        IFoodRecordRepository foodRecordRepository)
    {
        _foodRecordRepository = foodRecordRepository;
    }

    public async Task CreateAsync(CreateFoodRecordRequest request)
    {
        var foodRecord = new FoodRecord
        {
            Id = Guid.NewGuid(),
            MealType = request.MealType,
            FoodsJson = request.FoodsJson
        };

        await _foodRecordRepository.CreateAsync(foodRecord);
    }


    public async Task<List<FoodRecord>> GetFoodRecordsByDateAsync(
        DateOnly date)
    {
        return await _foodRecordRepository
            .GetFoodRecordsByDateAsync(date);
    }
}

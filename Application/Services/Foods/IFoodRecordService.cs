using Application.Services.Foods.Create;
using Domain.Entities.Foods;

namespace Application.Services.Foods;

public interface IFoodRecordService
{
    Task CreateAsync(CreateFoodRecordRequest request);
    Task<List<FoodRecord>> GetFoodRecordsByDateAsync(DateOnly date);
}

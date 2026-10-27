namespace Domain.Entities.Foods;

public interface IFoodRecordRepository
{
    Task CreateAsync(FoodRecord foodRecord);
    Task<List<FoodRecord>> GetFoodRecordsByDateAsync(DateOnly date);
}

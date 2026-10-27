using Domain.Entities.Foods;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;


public class FoodRecordRepository : IFoodRecordRepository
{
    private readonly AppDbContext _dbContext;

    public FoodRecordRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task CreateAsync(FoodRecord foodRecord)
    {
        await _dbContext.FoodRecords.AddAsync(foodRecord);
        await _dbContext.SaveChangesAsync();
    }


    public async Task<List<FoodRecord>> GetFoodRecordsByDateAsync(
        DateOnly date)
    {
        return await _dbContext.FoodRecords
            .AsNoTracking()
            .Where(record => record.CreatedAt == date)
            .OrderBy(record => record.MealType)
            .ToListAsync();
    }
}
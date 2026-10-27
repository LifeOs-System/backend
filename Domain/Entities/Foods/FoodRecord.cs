namespace Domain.Entities.Foods;

public class FoodRecord
{
    public Guid Id { get; set; }
    public MealType MealType { get; set; }
    public required string FoodsJson { get; set; }
    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}

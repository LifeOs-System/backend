using Domain.Entities.Foods;


namespace Application.Services.Foods.Create;

public class CreateFoodRecordRequest
{
    public MealType MealType { get; set; }
    public required string FoodsJson { get; set; }
}
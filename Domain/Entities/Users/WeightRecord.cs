namespace Domain.Entities.Users;

public class WeightRecord
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required decimal Weight { get; set; } // KG
    public required DateOnly Date { get; set; }
    public User? User { get; set; }
}
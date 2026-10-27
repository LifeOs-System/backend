namespace Domain.Entities.Users;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateOnly BirthDay { get; set; }
    public required decimal Weight { get; set; } // KG
    public required decimal Height { get; set; } // CM
    public required decimal BodyFatPercentage { get; set; }
    public required decimal BMI { get; set; }
    public required Gender Gender { get; set; }
    public List<WeightRecord> WeightRecords { get; set; } = new ();
    public List<ProgressPhoto> ProgressPhotos { get; set; } = new();
    public BodyMetrics? BodyMetrics { get; set; }
    public int Age { get; set; }
}

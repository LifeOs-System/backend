namespace Domain.Entities.Users;

public class BodyMetrics
{
    public Guid Id { get; set; }

    public required Guid UserId { get; set; }

    public required decimal Neck { get; set; } // CM
    public required decimal Chest { get; set; } // CM
    public required decimal Waist { get; set; } // CM
    public required decimal Hips { get; set; } // CM

    public required decimal LeftArm { get; set; } // CM
    public required decimal RightArm { get; set; } // CM

    public required decimal LeftForearm { get; set; } // CM
    public required decimal RightForearm { get; set; } // CM

    public required decimal LeftThigh { get; set; } // CM
    public required decimal RightThigh { get; set; } // CM

    public required decimal LeftCalf { get; set; } // CM
    public required decimal RightCalf { get; set; } // CM

    public required DateOnly Date { get; set; }

    public User? User { get; set; }
}
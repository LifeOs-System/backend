namespace Application.Services.Users.Create;

public class BodyMetricsRequest
{
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
}

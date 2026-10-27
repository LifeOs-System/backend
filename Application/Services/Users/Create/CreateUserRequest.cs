using Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Users.Create;

public class CreateUserRequest
{
    public required string Name { get; set; }
    public required DateOnly BirthDay { get; set; }
    public required decimal Weight { get; set; } // KG
    public required decimal Height { get; set; } // CM
    public required Gender Gender { get; set; }
    public List<IFormFile> Images { get; set; } = new();
    public required decimal BodyFatPercentage { get; set; }
    public required decimal BMI { get; set; }
    public BodyMetricsRequest? BodyMetricsRequest { get; set; }
    public int Age { get; set; }
}

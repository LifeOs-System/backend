namespace Domain.Entities.Users;

public class ProgressPhoto
{
    public string Id { get; set; } // ID of the progress photo | Key in Minio
    public required Guid UserId { get; set; }
    public required DateOnly Date { get; set; }
    public User? User { get; set; }
}
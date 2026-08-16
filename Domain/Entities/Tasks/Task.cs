namespace Domain.Entities.Tasks;

public class Task
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsCompleted { get; set; }
    public DateOnly? Date { get; set; } = null;
}

namespace Domain.Entities.ToDoTask;

public class ToDoTask
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsCompleted { get; set; }
    public DateOnly? Date { get; set; } = null;
}

namespace Application.Services.ToDoTasks.Create;

public class CreateToDoTaskRequest
{
    public required string Name { get; set; }
    public DateOnly? Date { get; set; } = null;
}

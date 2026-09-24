namespace Application.Services.ToDoTasks.GetAll;

public class ToDoTaskResponse
{
    public required string Name { get; set; }
    public DateOnly? Date { get; set; }
    public required string Id { get; set; }
}

using Application.Services.ToDoTasks.Create;
using Application.Services.ToDoTasks.GetAll;



namespace Application.Services.ToDoTasks;


public interface IToDoTaskService
{
    Task CreateAsync(CreateToDoTaskRequest req);
    Task<string?> CompleteAsync(Guid id);
    Task<List<ToDoTaskResponse>> GetAllAsync();
    Task<string?> DeleteAsync(Guid id);
}
namespace Domain.Entities.ToDoTask;


public interface IToDoTaskRepository
{
    Task CreateAsync(ToDoTask task);
    Task CompleteAsync(ToDoTask task);
    Task<List<ToDoTask>> GetAllAsync();
    Task DeleteAsync(ToDoTask task);
    Task<ToDoTask?> GetByIdAsync(Guid id);
}

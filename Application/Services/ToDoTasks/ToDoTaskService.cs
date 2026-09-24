using Application.Services.ToDoTasks.Create;
using Application.Services.ToDoTasks.GetAll;
using Domain.Entities.ToDoTask;

namespace Application.Services.ToDoTasks;

public class ToDoTaskService : IToDoTaskService
{
    private readonly IToDoTaskRepository _repository;

    public ToDoTaskService(IToDoTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateAsync(CreateToDoTaskRequest req)
    {
        var task = new ToDoTask
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            IsCompleted = false,
            Date = req.Date
        };

        await _repository.CreateAsync(task);
    }

    public async Task<string?> CompleteAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task is null)
        {
            return "No se encontró la tarea";
        }

        await _repository.CompleteAsync(task);
        return null;
    }

    public async Task<List<ToDoTaskResponse>> GetAllAsync()
    {
        var tasks = await _repository.GetAllAsync();

        // 2. Mapeamos manualmente de Entidad a DTO
        return tasks.Select(task => new ToDoTaskResponse
        {
            Id = task.Id.ToString(),
            Name = task.Name,
            Date = task.Date
        }).ToList();
    }


    public async Task<string?> DeleteAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task is null)
        {
            return "No se encontró la tarea";
        }

        await _repository.DeleteAsync(task);
        return null;
    }
}
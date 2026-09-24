using Domain.Entities.ToDoTask;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories;

public class ToDoTaskRepository : IToDoTaskRepository
{
    private readonly AppDbContext _dbContext;

    public ToDoTaskRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CompleteAsync(ToDoTask task)
    {
        task.IsCompleted = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateAsync(ToDoTask task)
    {
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ToDoTask task)
    {
        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<ToDoTask>> GetAllAsync()
    {
        // Calculamos la fecha límite: hoy + 3 días
        var maxFutureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3));

        return await _dbContext.Tasks
            .Where(task => task.Date == null || task.Date <= maxFutureDate)

            .OrderBy(t => t.Date == null)

            .ThenBy(t => t.Date)

            .ToListAsync();
    }

    public async Task<ToDoTask?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
    }
}

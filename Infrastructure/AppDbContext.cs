using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;
using Microsoft.EntityFrameworkCore;

using Task = Domain.Entities.Tasks.Task;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitRecord> HabitRecords => Set<HabitRecord>();
    public DbSet<Task> Tasks => Set<Task>();

}
using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;
using Domain.Entities.ToDoTask;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitRecord> HabitRecords => Set<HabitRecord>();
    public DbSet<ToDoTask> Tasks => Set<ToDoTask>();
}
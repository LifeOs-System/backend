using Domain.Entities.Books;
using Domain.Entities.Foods;
using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;
using Domain.Entities.ToDoTask;
using Domain.Entities.Users;
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
    public DbSet<Book> Books => Set<Book>();
    public DbSet<User> Users => Set<User>();
    public DbSet<WeightRecord> WeightRecords => Set<WeightRecord>();
    public DbSet<ProgressPhoto> ProgressPhotos => Set<ProgressPhoto>();
    public DbSet<FoodRecord> FoodRecords => Set<FoodRecord>();
}
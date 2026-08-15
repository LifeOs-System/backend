using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;


namespace Application.Services.Background;

public class HabitRecordCreationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HabitRecordCreationService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public HabitRecordCreationService(
        IServiceProvider serviceProvider,
        ILogger<HabitRecordCreationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HabitRecordCreationService iniciado");

        // Ejecutar inmediatamente al iniciar
        await CreateHabitRecordsForToday(stoppingToken);

        // Luego ejecutar cada hora
        using var timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CreateHabitRecordsForToday(stoppingToken);
        }
    }

    private async Task CreateHabitRecordsForToday(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var dayOfWeek = DateTime.Today.DayOfWeek;

            _logger.LogInformation("Creando HabitRecords para {Date} ({DayOfWeek})", today, dayOfWeek);

            // Obtener todos los hábitos activos
            var habits = await context.Habits
                .Where(h => h.Status != HabitStatus.Deleted)
                .ToListAsync(stoppingToken);

            var createdCount = 0;

            foreach (var habit in habits)
            {
                // Verificar si el hábito aplica para este día de la semana
                if (!ShouldCreateForDay(habit, dayOfWeek))
                {
                    continue;
                }

                // Verificar si ya existe un registro para hoy
                var exists = await context.HabitRecords
                    .AnyAsync(hr => hr.HabitId == habit.Id && hr.Date == today, stoppingToken);

                if (exists)
                {
                    continue;
                }

                // Crear el registro
                var record = new HabitRecord
                {
                    Id = Guid.NewGuid(),
                    HabitId = habit.Id,
                    Date = today,
                    Value = null,
                    Target = habit.Type != HabitType.Binary ? habit.Target : null,
                    IsCompleted = false,
                    Habit = habit
                };

                context.HabitRecords.Add(record);
                createdCount++;
            }

            if (createdCount > 0)
            {
                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Creados {Count} HabitRecords para {Date}", createdCount, today);
            }
            else
            {
                _logger.LogInformation("No se crearon nuevos HabitRecords para {Date}", today);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear HabitRecords");
        }
    }

    private bool ShouldCreateForDay(Habit habit, DayOfWeek dayOfWeek)
    {
        return habit.Days.Any(d => d == dayOfWeek);
    }
}
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

        await CreateHabitRecordsForToday(stoppingToken);

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

            // 1. Obtener hábitos activos
            var habits = await context.Habits
                .Where(h => h.Status == HabitStatus.Active) // Mejor usar == Active que != Deleted
                .ToListAsync(stoppingToken);

            if (!habits.Any()) return;

            var habitIds = habits.Select(h => h.Id).ToList();

            // 2. OPTIMIZACIÓN: Obtener TODOS los registros de hoy de una sola vez (Evita el problema N+1)
            var existingRecordsToday = await context.HabitRecords
                .Where(hr => hr.Date == today && habitIds.Contains(hr.HabitId))
                .Select(hr => hr.HabitId)
                .ToHashSetAsync(stoppingToken);

            var recordsToCreate = new List<HabitRecord>();

            foreach (var habit in habits)
            {
                // ✅ 3. Lógica corregida: Ahora incluye hábitos de frecuencia
                if (!ShouldCreateForDay(habit, dayOfWeek))
                {
                    continue;
                }

                // 4. Verificar en memoria (O(1)) en lugar de hacer una consulta a la BD por cada hábito
                if (existingRecordsToday.Contains(habit.Id))
                {
                    continue;
                }

                // 5. Crear el registro en memoria
                recordsToCreate.Add(new HabitRecord
                {
                    Id = Guid.NewGuid(),
                    HabitId = habit.Id,
                    Date = today,
                    Value = null,
                    Target = habit.Type != HabitType.Binary ? habit.Target : null,
                    IsCompleted = false
                    // ⚠️ IMPORTANTE: No asignar 'Habit = habit'. EF Core ya tiene la relación por HabitId.
                    // Asignar el objeto de navegación aquí puede causar excepciones de rastreo al hacer SaveChanges.
                });
            }

            if (recordsToCreate.Any())
            {
                await context.HabitRecords.AddRangeAsync(recordsToCreate, stoppingToken);
                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("✅ Creados {Count} HabitRecords para {Date}", recordsToCreate.Count, today);
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

    // ✅ MÉTODO CORREGIDO
    private bool ShouldCreateForDay(Habit habit, DayOfWeek dayOfWeek)
    {
        // 1. Si tiene días específicos configurados, solo aplica si hoy es uno de esos días
        if (habit.Days != null && habit.Days.Any())
        {
            return habit.Days.Contains(dayOfWeek);
        }

        // 2. Si es un hábito de frecuencia (Semanal/Mensual), está disponible para crearse CUALQUIER día.
        // (La validación de si el usuario ya alcanzó el límite de ocurrencias de la semana/mes 
        // se maneja en el servicio GetHabitsTodayAsync, no aquí).
        if (habit.Frequency != null && habit.Occurrences.HasValue && habit.Occurrences > 0)
        {
            return true;
        }

        // 3. Si no tiene días ni frecuencia configurada, no se crea automáticamente
        return false;
    }
}
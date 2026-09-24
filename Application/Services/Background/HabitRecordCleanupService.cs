using Domain.Entities.Habits;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services.Background;


public class HabitRecordCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HabitRecordCleanupService> _logger;

    // ⏱️ CAMBIO: Se ejecuta cada 2 horas para pruebas
    private readonly TimeSpan _interval = TimeSpan.FromHours(2);

    public HabitRecordCleanupService(
        IServiceProvider serviceProvider,
        ILogger<HabitRecordCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HabitRecordCleanupService iniciado (Intervalo: 2 horas)");

        // Ejecutar una vez al iniciar la aplicación
        await CleanupIncompleteRecords(stoppingToken);

        // Luego ejecutar cada 2 horas
        using var timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanupIncompleteRecords(stoppingToken);
        }
    }

    private async Task CleanupIncompleteRecords(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // ⚠️ NOTA PARA PRUEBAS: 
            // Actualmente busca registros de AYER. 
            // Si quieres probar con registros de HOY, cambia AddDays(-1) por AddDays(0)
            var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

            _logger.LogInformation("Iniciando limpieza de registros incompletos para la fecha: {Date}", targetDate);

            // 1. Obtener los IDs de los hábitos que usan Frecuencia/Ocurrencias y están activos
            var habitIdsWithFrequency = await context.Habits
                .Where(h => h.Status != HabitStatus.Deleted && h.Occurrences != null) // Ajusta esta condición si usas otra lógica
                .Select(h => h.Id)
                .ToListAsync(stoppingToken);

            if (!habitIdsWithFrequency.Any())
            {
                _logger.LogInformation("No se encontraron hábitos con frecuencia configurada. Limpieza omitida.");
                return;
            }

            // 2. Buscar los registros de la fecha objetivo que pertenezcan a esos hábitos y NO estén completados
            var recordsToDelete = await context.HabitRecords
                .Where(hr => hr.Date == targetDate &&
                             habitIdsWithFrequency.Contains(hr.HabitId) &&
                             hr.IsCompleted == false)
                .ToListAsync(stoppingToken);

            if (recordsToDelete.Any())
            {
                // 3. Eliminarlos en lote
                context.HabitRecords.RemoveRange(recordsToDelete);
                await context.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("✅ Eliminados {Count} registros incompletos de hábitos con frecuencia para la fecha {Date}",
                    recordsToDelete.Count, targetDate);
            }
            else
            {
                _logger.LogInformation("No se encontraron registros incompletos para limpiar en la fecha {Date}", targetDate);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico al limpiar registros de hábitos incompletos");
        }
    }
}
using Api.Endoints;
using Api.Endpoints;
using Application.Services.Background;
using Application.Services.Books;
using Application.Services.HabitRecords;
using Application.Services.Habits;
using Application.Services.ToDoTasks;
using Domain.Entities.Books;
using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;
using Domain.Entities.ToDoTask;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// PostgreSQL / EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
});

// Repositories
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitRecordRepository, HabitRecordRepository>();
builder.Services.AddScoped<IToDoTaskRepository, ToDoTaskRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

// Services
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IHabitRecordService, HabitRecordService>();
builder.Services.AddScoped<IToDoTaskService, ToDoTaskService>();
builder.Services.AddScoped<IBookService, BookService>();

// 1. Registra la clase concreta como Singleton para que HabitService pueda inyectarla
builder.Services.AddSingleton<HabitRecordCreationService>();

// 2. Registra el BackgroundService usando esa misma instancia (no crea una nueva)
builder.Services.AddHostedService(sp => sp.GetRequiredService<HabitRecordCreationService>());
builder.Services.AddHostedService<HabitRecordCleanupService>();

var app = builder.Build();

# region Aplicar Migraciones de Base de Datos al Iniciar la Aplicación
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        logger.LogInformation("Migraciones de base de datos aplicadas correctamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones de la base de datos.");
        throw; 
    }
}
#endregion

app.UseCors("AllowAll");
// Endpoints
HabitEndpoints.Map(app);
HabitRecordEndpoint.Map(app);
TaskEndpoint.Map(app);
BookEndpoint.Map(app);

app.Run();
using Api.Endoints;
using Api.Endpoints;
using Application.Services.Background;
using Application.Services.Blob;
using Application.Services.Blob.Minio;
using Application.Services.Books;
using Application.Services.Foods;
using Application.Services.HabitRecords;
using Application.Services.Habits;
using Application.Services.ToDoTasks;
using Application.Services.Users;
using Domain.Entities.Books;
using Domain.Entities.Foods;
using Domain.Entities.Habits;
using Domain.Entities.HabitsRecords;
using Domain.Entities.ToDoTask;
using Domain.Entities.Users;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
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


#region MinIO
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection(MinioSettings.SectionName));

builder.Services.AddSingleton<IMinioClient>(provider =>
{
    var minioOptions = provider.GetRequiredService<IOptions<MinioSettings>>().Value;

    // Configuración del cliente MinIO
    var minioClient = new MinioClient()
        .WithEndpoint(minioOptions.Endpoint)
        .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
        .WithSSL(minioOptions.UseSsl)
        .Build();

    return minioClient;
});

builder.Services.AddScoped<IBlobService, MinioBlobService>();
#endregion


// Repositories
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitRecordRepository, HabitRecordRepository>();
builder.Services.AddScoped<IToDoTaskRepository, ToDoTaskRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFoodRecordRepository, FoodRecordRepository>();

// Services
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IHabitRecordService, HabitRecordService>();
builder.Services.AddScoped<IToDoTaskService, ToDoTaskService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFoodRecordService, FoodRecordService>();

builder.Services.AddAntiforgery();

// 1. Registra la clase concreta como Singleton para que HabitService pueda inyectarla
builder.Services.AddSingleton<HabitRecordCreationService>();

// 2. Registra el BackgroundService usando esa misma instancia (no crea una nueva)
builder.Services.AddHostedService(sp => sp.GetRequiredService<HabitRecordCreationService>());
builder.Services.AddHostedService<HabitRecordCleanupService>();

var app = builder.Build();

app.UseAntiforgery();

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
UserEndpoints.Map(app);
FoodRecordEndpoint.Map(app);

app.Run();
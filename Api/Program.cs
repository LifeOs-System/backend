using Api.Endoints;
using Api.Endpoints;
using Application.Services.Background;
using Application.Services.HabitRecords;
using Application.Services.Habits;
using Application.Services.ToDoTasks;
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

// Services
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IHabitRecordService, HabitRecordService>();
builder.Services.AddScoped<IToDoTaskService, ToDoTaskService>();

//Background Services
builder.Services.AddHostedService<HabitRecordCreationService>();
builder.Services.AddHostedService<HabitRecordCleanupService>();

var app = builder.Build();

app.UseCors("AllowAll");
// Endpoints
HabitEndpoints.Map(app);
HabitRecordEndpoint.Map(app);
TaskEndpoint.Map(app);

app.Run();
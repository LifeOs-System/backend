using Application.Services.ToDoTasks;
using Application.Services.ToDoTasks.Create;

namespace Api.Endpoints;

public static class TaskEndpoint
{
    public static void Map(WebApplication app)
    {
        // 1. Obtener todas las tareas
        app.MapGet("/api/todotasks", async (IToDoTaskService taskService) =>
        {
            var result = await taskService.GetAllAsync();
            return Results.Ok(result);
        });

        // 2. Crear una nueva tarea
        app.MapPost("/api/todotasks", async (CreateToDoTaskRequest request, IToDoTaskService taskService) =>
        {
            await taskService.CreateAsync(request);

            // Nota: Idealmente tu servicio debería devolver la tarea creada para hacer un "Results.Created"
            // Pero con tu firma actual, devolvemos 200 OK o 201 sin ubicación.
            return Results.Ok(new { message = "Tarea creada exitosamente" });
        });

        // 3. Completar una tarea (Usamos POST o PUT para acciones sobre un recurso)
        app.MapPost("/api/todotasks/{id:guid}/complete", async (Guid id, IToDoTaskService taskService) =>
        {
            var error = await taskService.CompleteAsync(id);

            // Si el servicio devuelve un string, significa que hubo un error (ej: "No se encontró la tarea")
            if (error is not null)
            {
                return Results.NotFound(new { message = error });
            }

            // 204 No Content es la respuesta estándar cuando una operación de modificación es exitosa y no devuelve datos
            return Results.NoContent();
        });

        // 4. Eliminar una tarea
        app.MapDelete("/api/todotasks/{id:guid}", async (Guid id, IToDoTaskService taskService) =>
        {
            var error = await taskService.DeleteAsync(id);

            if (error is not null)
            {
                return Results.NotFound(new { message = error });
            }

            return Results.NoContent();
        });
    }
}

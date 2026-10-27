using Application.Services.Users;
using Application.Services.Users.Create;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class UserEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/user", async (
    HttpRequest httpRequest,
    [FromForm] CreateUserRequest request,
    [FromServices] IUserService userService) =>
        {
            var form = await httpRequest.ReadFormAsync();

            request.Images = form.Files.GetFiles("Images").ToList();

            await userService.CreateUserAsync(request, default);

            return Results.Ok();
        })
.DisableAntiforgery();
    }
}

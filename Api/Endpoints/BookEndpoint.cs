using Application.Services.Books;
using Application.Services.Books.Create;
using Application.Services.Books.Update;


namespace Api.Endpoints;

public class BookEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/book", async (
            CreateBookRequest request,
            IBookService bookService) =>
        {
            await bookService.CreateAsync(request);

            return Results.Ok();
        });

        app.MapGet("/api/books", async (
            IBookService bookService) =>
        {
            var books = await bookService.GetAllAsync();
            return Results.Ok(books);
        });

        app.MapPatch("/api/books/{id:guid}", async (
            Guid id,
            UpdateBookSummaryRequest request,
            IBookService bookService) =>
        {
            await bookService.UpdateSummaryAsync(id, request);
            return Results.NoContent();
        });

    }
}

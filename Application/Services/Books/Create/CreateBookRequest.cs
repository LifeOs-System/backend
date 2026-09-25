namespace Application.Services.Books.Create;

public class CreateBookRequest
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public bool IsRead { get; set; }
    public DateOnly? ReadDate { get; set; }
    public string? SummaryMarkdown { get; set; }
    public int? Rating { get; set; }
}

namespace Application.Services.Books.GetAll;

public class BookResponse
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateOnly? ReadDate { get; set; }
    public DateOnly CreatedAt { get; set; }
    public string? SummaryMarkdown { get; set; }
    public int? Rating { get; set; }
    public int Order { get; set; }
}
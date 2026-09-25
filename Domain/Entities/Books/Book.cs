namespace Domain.Entities.Books;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Author { get; set; }
    public bool IsRead { get; set; } = false;
    public DateOnly? ReadDate { get; set; }
    public string? SummaryMarkdown { get; set; }
    public int? Rating { get; set; }
    public int Order { get; set; }
}
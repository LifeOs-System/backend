namespace Domain.Entities.Books;

public interface IBookRepository
{
    Task CreateAsync(Book book);
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(Guid id);
    Task UpdateAsync(Book book);
}

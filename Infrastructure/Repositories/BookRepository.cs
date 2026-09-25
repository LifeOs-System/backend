using Domain.Entities.Books;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _dbContext;

    public BookRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Book book)
    {
        var maxOrder = await _dbContext.Books.MaxAsync(b => (int?)b.Order) ?? 0;

        book.Order = maxOrder + 1;

        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _dbContext.Books
            .OrderBy(b => b.Order)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Books.FindAsync(id);
    }


    public async Task UpdateAsync(Book book)
    {
        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync();
    }
}

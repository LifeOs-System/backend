using Application.Services.Books.Create;
using Application.Services.Books.GetAll;
using Application.Services.Books.Update;
using Domain.Entities.Books;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Books;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task CreateAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            IsRead = request.IsRead,
            ReadDate = request.ReadDate,
            SummaryMarkdown = request.SummaryMarkdown,
            Rating = request.Rating
        };

        // 4. Delegar la persistencia al repositorio
        await _bookRepository.CreateAsync(book);
    }

    public async Task<List<BookResponse>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync();

        return books.Select(b => new BookResponse
        {
            Id = b.Id.ToString(),
            Title = b.Title,
            Author = b.Author,
            IsRead = b.IsRead,
            ReadDate = b.ReadDate,
            SummaryMarkdown = b.SummaryMarkdown,
            Rating = b.Rating,
            Order = b.Order
        }).ToList();
    }

    public async Task UpdateSummaryAsync(Guid id, UpdateBookSummaryRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
        {
            throw new KeyNotFoundException($"No se encontró un libro con ID {id}");
        }

        book.SummaryMarkdown = request.SummaryMarkdown;

        await _bookRepository.UpdateAsync(book);
    }
}


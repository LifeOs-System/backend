using Application.Services.Books.Create;
using Application.Services.Books.GetAll;
using Application.Services.Books.Update;

namespace Application.Services.Books;


public interface IBookService
{
    Task CreateAsync(CreateBookRequest request);
    Task<List<BookResponse>> GetAllAsync();
    Task UpdateSummaryAsync(Guid id, UpdateBookSummaryRequest request);
}
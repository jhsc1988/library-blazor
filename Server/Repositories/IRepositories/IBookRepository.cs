using lib_blazor.Models;

namespace lib_blazor.Server.Repositories.IRepositories;

public interface IBookRepository
{
    Task<(bool IsSuccess, IEnumerable<Book> Books, string ErrorMessage)> GetBooksAsync(string searchTerm = null);

    Task<(bool IsSuccess, Book Book, string ErrorMessage)> GetBookByIdAsync(int id);
    Task<(bool IsSuccess, Book Book, string ErrorMessage)> GetOriginalBookByIdAsync(int id);
    Task<(bool IsSuccess, int ReservedCount, string ErrorMessage)> GetReservedCountForBookAsync(int id);
    Task<(bool IsSuccess, string ErrorMessage)> BookExistsAsync(int id);
    Task<(bool IsSuccess, string ErrorMessage)> UpdateBookAsync(Book book);
    Task<(bool IsSuccess, string ErrorMessage)> AddBookAsync(Book book);
    Task<(bool IsSuccess, string ErrorMessage)> DeleteBookAsync(Book book);
}
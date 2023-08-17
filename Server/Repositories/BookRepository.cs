using lib_blazor.Models;
using lib_blazor.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace lib_blazor.Business;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BookRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Book>> GetBooksAsync()
    {
        return await _dbContext.Books.ToListAsync();
    }

    public async Task UploadBookAsync(Book book)
    {
        await _dbContext.Books.AddAsync(book); //TODO: BookDTO
        await _dbContext.SaveChangesAsync();
    }
}

public interface IBookRepository
{
    public Task UploadBookAsync(Book book);
    public Task<List<Book>> GetBooksAsync();
}
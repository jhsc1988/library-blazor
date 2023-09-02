using Microsoft.Extensions.Logging;

namespace lib_blazor.Server.Repositories
{
    using Model;
    using IRepositories;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(ApplicationDbContext context, ILogger<BookRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, IEnumerable<Book> Books, string ErrorMessage)> GetBooksAsync(
            string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation($"Attempting to get books with searchTerm: {searchTerm}");

                var query = _context.Books.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(b => b.Title.ToLower().Contains(searchTerm.ToLower()) ||
                                             b.Author.ToLower().Contains(searchTerm.ToLower()) ||
                                             b.Annotation.ToLower().Contains(searchTerm.ToLower()));
                }

                var books = await query.ToListAsync();
                return (true, books, null)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get books with searchTerm: {searchTerm}. Error: {ex.Message}");
                return (false, null, ex.Message)!;
            }
        }

        public async Task<(bool IsSuccess, Book Book, string ErrorMessage)> GetBookByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to get book by ID: {id}");

                var book = await _context.Books.FindAsync(id);
                return book != null ? (true, book, null)! : (false, null, "Book not found")!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get book by ID: {id}. Error: {ex.Message}");
                return (false, null, ex.Message)!;
            }
        }

        public async Task<(bool IsSuccess, Book Book, string ErrorMessage)> GetOriginalBookByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to get original book by ID: {id}");

                var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                return book != null ? (true, book, null)! : (false, null, "Book not found")!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get original book by ID: {id}. Error: {ex.Message}");
                return (false, null, ex.Message)!;
            }
        }

        public async Task<(bool IsSuccess, int ReservedCount, string ErrorMessage)> GetReservedCountForBookAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to get reserved count for book ID: {id}");

                var count = await _context.Reservations.CountAsync(r => r.Book!.Id == id);
                return (true, count, null)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get reserved count for book ID: {id}. Error: {ex.Message}");
                return (false, 0, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> BookExistsAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Checking if book with ID: {id} exists");

                var exists = await _context.Books.AnyAsync(e => e.Id == id);
                return (exists, null)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to check existence for book ID: {id}. Error: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateBookAsync(Book book)
        {
            try
            {
                _logger.LogInformation($"Attempting to update book with ID: {book.Id}");

                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return (true, null)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update book with ID: {book.Id}. Error: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddBookAsync(Book book)
        {
            try
            {
                _logger.LogInformation($"Attempting to add a new book");

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return (true, null)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add a new book. Error: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteBookAsync(Book book)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete book with ID: {book.Id}");

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return (true, null)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete book with ID: {book.Id}. Error: {ex.Message}");
                return (false, ex.Message);
            }
        }
    }
}
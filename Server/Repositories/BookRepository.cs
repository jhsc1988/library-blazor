namespace lib_blazor.Server.Repositories;

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

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool IsSuccess, IEnumerable<Book> Books, string ErrorMessage)> GetBooksAsync(
        string? searchTerm = null)
    {
        try
        {
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
            return (false, null, ex.Message)!;
        }
    }


    public async Task<(bool IsSuccess, Book Book, string ErrorMessage)> GetBookByIdAsync(int id)
    {
        try
        {
            var book = await _context.Books.FindAsync(id);
            return book != null ? (true, book, null)! : (false, null, "Book not found")!;
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message)!;
        }
    }

    public async Task<(bool IsSuccess, Book Book, string ErrorMessage)> GetOriginalBookByIdAsync(int id)
    {
        try
        {
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            return book != null ? (true, book, null)! : (false, null, "Book not found")!;
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message)!;
        }
    }

    public async Task<(bool IsSuccess, int ReservedCount, string ErrorMessage)> GetReservedCountForBookAsync(int id)
    {
        try
        {
            var count = await _context.Reservations.CountAsync(r => r.Book!.Id == id);
            return (true, count, null)!;
        }
        catch (Exception ex)
        {
            return (false, 0, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> BookExistsAsync(int id)
    {
        try
        {
            var exists = await _context.Books.AnyAsync(e => e.Id == id);
            return (exists, null)!;
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> UpdateBookAsync(Book book)
    {
        try
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return (true, null)!;
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> AddBookAsync(Book book)
    {
        try
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return (true, null)!;
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> DeleteBookAsync(Book book)
    {
        try
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return (true, null)!;
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
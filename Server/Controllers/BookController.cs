using lib_blazor.Server.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using lib_blazor.Model;
using lib_blazor.Server.Repositories.IRepositories;

namespace lib_blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: api/Book/5/edit
        [HttpGet("{id}/edit")]
        public async Task<ActionResult<Model.Book>> GetBookForEditing(int id)
        {
            var result = await _bookRepository.GetOriginalBookByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return result.Book;
        }

// GET: api/Book/5/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<Model.Book>> GetBookDetails(int id)
        {
            var result = await _bookRepository.GetBookByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            var reservedResult = await _bookRepository.GetReservedCountForBookAsync(id);
            if (!reservedResult.IsSuccess)
            {
                return StatusCode(500, reservedResult.ErrorMessage);
            }

            result.Book.Amount -= reservedResult.ReservedCount;

            if (result.Book.Amount <= 0) result.Book.Amount = 0;

            return result.Book;
        }

        // GET: api/Book
        [HttpGet]
        // GET: api/Book/search?searchTerm=xyz
        [HttpGet("search")]
        public async Task<IActionResult> GetBooks(string? searchTerm)
        {
            var result = await _bookRepository.GetBooksAsync(searchTerm);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Books);
        }

        // GET: api/Book/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Model.Book>> GetBook(int id, bool forEditing = false)
        {
            (bool IsSuccess, Model.Book Book, string ErrorMessage) result;

            // Decide which method to use based on the forEditing flag.
            if (forEditing)
            {
                result = await _bookRepository.GetOriginalBookByIdAsync(id);
            }
            else
            {
                result = await _bookRepository.GetBookByIdAsync(id);
                if (result.IsSuccess)
                {
                    var reservedResult = await _bookRepository.GetReservedCountForBookAsync(id);
                    if (!reservedResult.IsSuccess)
                    {
                        return StatusCode(500, reservedResult.ErrorMessage);
                    }

                    result.Book.Amount -= reservedResult.ReservedCount;

                    if (result.Book.Amount <= 0) result.Book.Amount = 0;
                }
            }

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return result.Book;
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            var result = await _bookRepository.UpdateBookAsync(book);
            if (!result.IsSuccess)
            {
                if (!(await _bookRepository.BookExistsAsync(id)).IsSuccess)
                {
                    return NotFound();
                }
                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }

        // POST: api/Book
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            var result = await _bookRepository.AddBookAsync(book);
            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var currentUser = User;
            bool isAdmin = currentUser.HasClaim(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Admin");

            if (!isAdmin)
            {
                return Forbid();
            }

            var bookResult = await _bookRepository.GetBookByIdAsync(id);
            if (!bookResult.IsSuccess)
            {
                return NotFound(new { message = "Book not found" });
            }

            var deleteResult = await _bookRepository.DeleteBookAsync(bookResult.Book);
            if (!deleteResult.IsSuccess)
            {
                return StatusCode(500, deleteResult.ErrorMessage);
            }

            return NoContent();
        }
    }
}

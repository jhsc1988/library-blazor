using System.Diagnostics;
using System.Security.Claims;
using lib_blazor.Model;
using lib_blazor.Server.Data;
using lib_blazor.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lib_blazor.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ReserveBook([FromBody] int bookId)
        {
            Console.WriteLine("ReserveBook called");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            var book = await _context.Books.FindAsync(bookId);
            var effectiveAmount = await GetEffectiveAmount(bookId);
            if (book == null || effectiveAmount <= 0)
            {
                return BadRequest("No more copies of the book available for reservation.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Check if the user has already reserved this book
            var existingReservation = await _context.Reservations
                .AnyAsync(r => r.Book!.Id == bookId && r.User!.Id == userId);

            if (existingReservation)
            {
                return BadRequest("You have already reserved this book.");
            }

            var reservation = new Reservation
            {
                Book = book,
                User = user
            };

            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();

            return Ok("Book reserved successfully!");
        }

        
        public async Task<int> GetEffectiveAmount(int bookId)
        {
            var totalReservations = await _context.Reservations.CountAsync(r => r.Book!.Id == bookId);
            var book = await _context.Books.FindAsync(bookId);
            Debug.Assert(book != null, nameof(book) + " != null");
            return book.Amount - totalReservations;
        }
        
        [HttpGet]
        public  IEnumerable<ReservationDto> GetAllReservationsAsDtOs()
        {
            var currentUser = HttpContext.User;
            bool isAdmin = currentUser.HasClaim(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Admin");
            bool isUser = currentUser.HasClaim(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "User");
            
            if (isAdmin)
            {
                var reservations = _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Book)
                    .Select(r => new ReservationDto
                    {
                        Username = r.User!.UserName ?? string.Empty,
                        BookTitle = r.Book!.Title,
                        Author = r.Book.Author,
                        ReservationId = r.Id
                    })
                    .ToList();

                return reservations;
            }

            if (isUser)
            {
                var userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

                var reservations = _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Book)
                    .Where(r => r.User!.Id == userId) // Filter by the current user's ID
                    .Select(r => new ReservationDto
                    {
                        Username = r.User!.UserName ?? string.Empty,
                        BookTitle = r.Book!.Title,
                        Author = r.Book.Author,
                        ReservationId = r.Id
                    })
                    .ToList();

                return reservations;
            }
            else
            {
                return new List<ReservationDto>();
            }
        }
        
        [HttpDelete("{reservationId}")]
        public async Task<IActionResult> DeleteReservation(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.HasClaim(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Admin");
            if (!isAdmin && reservation.User?.Id != userId)
            {
                return Forbid();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok("Reservation deleted successfully!");
        }
    }
}
using System.Security.Claims;
using lib_blazor.Server.Data;
using lib_blazor.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace lib_blazor.Server.Controllers
{
    [ApiController]
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            var book = await _context.Books.FindAsync(bookId);
            var user = await _context.Users.FindAsync(userId);

            var reservation = new Reservation
            {
                Book = book,
                User = user
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok("Book reserved successfully!");
        }
    }
}
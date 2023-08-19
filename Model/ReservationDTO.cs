namespace lib_blazor.Model
{
    public class ReservationDto
    {
        public required string Username { get; init; }
        public required string BookTitle { get; init; }
        public required string Author { get; init; }
        public int ReservationId { get; init; }
    }
}
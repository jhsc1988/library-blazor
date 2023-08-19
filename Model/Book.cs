namespace lib_blazor.Model;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Publisher { get; set; }
    public required string Language { get; set; }
    public required string Annotation { get; set; }
    public required string Isbn { get; set; }
    public int Amount { get; set; }
    public required string CoverImageUrl { get; set; } 
}

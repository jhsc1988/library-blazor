namespace lib_blazor.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Publisher { get; set; }
    public string Language { get; set; }
    public string Annotation { get; set; }
    public string ISBN { get; set; }
    public int Amount { get; set; }
    public string CoverImageUrl { get; set; } 
}

namespace Gaby.io.ViewModels;

public class BookDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? PublisherName { get; set; }
    public List<string> GenreNames { get; set; } = new();
    public int PageCount { get; set; }
    public DateTime? PublicationDate { get; set; }
    public decimal? AverageRating { get; set; }
    public int TotalRatings { get; set; }
}
                                                                                                                                            
namespace Gaby.io.ViewModels;

public class BookDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string? PublisherName { get; set; }
    public List<string> GenreNames { get; set; } = new();
    public int PageCount { get; set; }
    public DateTime? PublicationDate { get; set; }
}
                                                                                                                                            
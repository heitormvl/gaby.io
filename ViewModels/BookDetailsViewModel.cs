namespace Gaby.io.ViewModels;

public class BookDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string? PublisherName { get; set; }
    public string? GenreName { get; set; }
    public int PageCount { get; set; }
    public DateTime? PublicationDate { get; set; }
}
                                                                                                                                            
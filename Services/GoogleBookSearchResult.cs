namespace Gaby.io.Services;

public class GoogleBookSearchResult
{
    public string Title { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string? PublisherName { get; set; }
    public int? PageCount { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? SuggestedGenreName { get; set; }
}

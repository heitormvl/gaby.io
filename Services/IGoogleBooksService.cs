namespace Gaby.io.Services;

public interface IGoogleBooksService
{
    Task<IReadOnlyList<GoogleBookSearchResult>> SearchAsync(string query, CancellationToken cancellationToken);
}

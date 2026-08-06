namespace Gaby.io.Services;

public interface IWikidataService
{
    Task<AuthorEnrichment?> LookupAuthorAsync(string authorName, CancellationToken cancellationToken);
}

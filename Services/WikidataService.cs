using System.Text.Json;

namespace Gaby.io.Services;

public class WikidataService : IWikidataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WikidataService> _logger;

    private static readonly string[] WriterKeywords =
    {
        "writer", "author", "novelist", "poet", "playwright", "essayist",
        "escritor", "escritora", "romancista", "poeta", "dramaturgo", "ensaísta"
    };

    // Wikidata QIDs for sex-or-gender (P21) values we can represent.
    private const string GenderMale = "Q6581097";
    private const string GenderFemale = "Q6581072";

    public WikidataService(HttpClient httpClient, ILogger<WikidataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AuthorEnrichment?> LookupAuthorAsync(string authorName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(authorName))
            return null;

        try
        {
            var qid = await FindAuthorEntityIdAsync(authorName, cancellationToken);
            if (qid == null)
                return null;

            var (genderQid, countryQid) = await GetClaimsAsync(qid, cancellationToken);
            if (genderQid == null && countryQid == null)
                return null;

            var enrichment = new AuthorEnrichment
            {
                Gender = MapGender(genderQid)
            };

            if (countryQid != null)
                enrichment.CountryName = await GetLabelAsync(countryQid, cancellationToken);

            return enrichment;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Falha ao consultar o Wikidata para o autor '{AuthorName}'", authorName);
            return null;
        }
    }

    private async Task<string?> FindAuthorEntityIdAsync(string authorName, CancellationToken cancellationToken)
    {
        var url = $"w/api.php?action=wbsearchentities&search={Uri.EscapeDataString(authorName)}&language=en&type=item&limit=5&format=json";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!doc.RootElement.TryGetProperty("search", out var results))
            return null;

        foreach (var result in results.EnumerateArray())
        {
            var description = result.TryGetProperty("description", out var descEl) ? descEl.GetString() ?? "" : "";
            if (WriterKeywords.Any(k => description.Contains(k, StringComparison.OrdinalIgnoreCase)))
            {
                return result.TryGetProperty("id", out var idEl) ? idEl.GetString() : null;
            }
        }

        // No candidate looked like a writer — skip rather than risk enriching the wrong person.
        return null;
    }

    private async Task<(string? genderQid, string? countryQid)> GetClaimsAsync(string qid, CancellationToken cancellationToken)
    {
        var url = $"w/api.php?action=wbgetentities&ids={qid}&props=claims&format=json";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return (null, null);

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!doc.RootElement.TryGetProperty("entities", out var entities) || !entities.TryGetProperty(qid, out var entity))
            return (null, null);

        if (!entity.TryGetProperty("claims", out var claims))
            return (null, null);

        var genderQid = ExtractEntityIdClaim(claims, "P21");
        var countryQid = ExtractEntityIdClaim(claims, "P27");
        return (genderQid, countryQid);
    }

    private static string? ExtractEntityIdClaim(JsonElement claims, string property)
    {
        if (!claims.TryGetProperty(property, out var claimArray) || claimArray.GetArrayLength() == 0)
            return null;

        try
        {
            return claimArray[0]
                .GetProperty("mainsnak")
                .GetProperty("datavalue")
                .GetProperty("value")
                .GetProperty("id")
                .GetString();
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    private async Task<string?> GetLabelAsync(string qid, CancellationToken cancellationToken)
    {
        var url = $"w/api.php?action=wbgetentities&ids={qid}&props=labels&languages=pt|en&format=json";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!doc.RootElement.TryGetProperty("entities", out var entities) || !entities.TryGetProperty(qid, out var entity))
            return null;

        if (!entity.TryGetProperty("labels", out var labels))
            return null;

        if (labels.TryGetProperty("pt", out var ptLabel) && ptLabel.TryGetProperty("value", out var ptValue))
            return ptValue.GetString();

        if (labels.TryGetProperty("en", out var enLabel) && enLabel.TryGetProperty("value", out var enValue))
            return enValue.GetString();

        return null;
    }

    private static char? MapGender(string? genderQid)
    {
        if (genderQid == null)
            return null;

        return genderQid switch
        {
            GenderMale => 'M',
            GenderFemale => 'F',
            _ => 'N'
        };
    }
}

using System.Globalization;
using Microsoft.Extensions.Options;

namespace Gaby.io.Services;

public class GoogleBooksService : IGoogleBooksService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleBooksOptions _options;
    private readonly ILogger<GoogleBooksService> _logger;

    public GoogleBooksService(HttpClient httpClient, IOptions<GoogleBooksOptions> options, ILogger<GoogleBooksService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<GoogleBookSearchResult>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Array.Empty<GoogleBookSearchResult>();

        var url = $"volumes?q=intitle:{Uri.EscapeDataString(query)}&langRestrict=pt&country=BR&maxResults=5";
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            url += $"&key={Uri.EscapeDataString(_options.ApiKey)}";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<GoogleBooksSearchResponse>(url, cancellationToken);
            if (response?.Items == null)
                return Array.Empty<GoogleBookSearchResult>();

            return response.Items
                .Select(i => i.VolumeInfo)
                .Where(v => v != null && !string.IsNullOrWhiteSpace(v.Title))
                .Select(v => new GoogleBookSearchResult
                {
                    Title = NormalizeTitle(v!.Title!),
                    AuthorName = v.Authors?.FirstOrDefault(),
                    PublisherName = v.Publisher,
                    PageCount = v.PageCount,
                    PublicationDate = ParsePublishedDate(v.PublishedDate),
                    SuggestedGenreName = TranslateGenreToPtBr(ExtractFirstCategorySegment(v.Categories?.FirstOrDefault()))
                })
                .ToList();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Falha ao consultar a API do Google Books para a busca '{Query}'", query);
            return Array.Empty<GoogleBookSearchResult>();
        }
    }

    private static DateTime? ParsePublishedDate(string? publishedDate)
    {
        if (string.IsNullOrWhiteSpace(publishedDate))
            return null;

        string[] formats = { "yyyy-MM-dd", "yyyy-MM", "yyyy" };
        return DateTime.TryParseExact(publishedDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed
            : null;
    }

    private static string? ExtractFirstCategorySegment(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return null;

        return category.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    }

    // Google Books' "categories" field always comes back in English (a fixed BISAC-style taxonomy),
    // regardless of langRestrict/country. Translate the common top-level ones to PT-BR; anything not
    // in this list is left in English rather than guessed at.
    private static readonly Dictionary<string, string> GenreTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Fiction"] = "Ficção",
        ["Nonfiction"] = "Não Ficção",
        ["Juvenile Fiction"] = "Ficção Juvenil",
        ["Young Adult Fiction"] = "Ficção Jovem Adulto",
        ["Biography & Autobiography"] = "Biografia e Autobiografia",
        ["History"] = "História",
        ["Poetry"] = "Poesia",
        ["Drama"] = "Drama",
        ["Religion"] = "Religião",
        ["Self-Help"] = "Autoajuda",
        ["Business & Economics"] = "Negócios e Economia",
        ["Humor"] = "Humor",
        ["Science Fiction"] = "Ficção Científica",
        ["Fantasy"] = "Fantasia",
        ["Mystery & Detective"] = "Mistério e Detetive",
        ["Thriller"] = "Suspense",
        ["Romance"] = "Romance",
        ["Cooking"] = "Culinária",
        ["Travel"] = "Viagem",
        ["Health & Fitness"] = "Saúde e Bem-Estar",
        ["Philosophy"] = "Filosofia",
        ["Psychology"] = "Psicologia",
        ["Science"] = "Ciência",
        ["Education"] = "Educação",
        ["Art"] = "Arte",
        ["Music"] = "Música",
        ["Sports & Recreation"] = "Esportes e Lazer",
        ["True Crime"] = "Crime Real",
        ["Comics & Graphic Novels"] = "Quadrinhos",
        ["Body, Mind & Spirit"] = "Corpo, Mente e Espírito",
        ["Brazilian fiction"] = "Ficção Brasileira",
        ["Domestics"] = "Empregados Domésticos"
    };

    private static string? TranslateGenreToPtBr(string? genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            return genre;

        return GenreTranslations.TryGetValue(genre.Trim(), out var translated) ? translated : genre;
    }

    private static readonly HashSet<string> LowercaseTitleWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "de", "da", "do", "das", "dos", "e", "a", "o", "as", "os", "em", "com", "para", "por"
    };

    // Google Books returns some editions with all-caps titles (e.g. "DOM CASMURRO"); convert those to
    // proper title case. Titles that already have mixed case are left untouched.
    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Any(char.IsLower))
            return title;

        var ptBr = CultureInfo.GetCultureInfo("pt-BR");
        var words = ptBr.TextInfo.ToTitleCase(title.ToLower(ptBr)).Split(' ');

        for (var i = 1; i < words.Length; i++)
        {
            if (LowercaseTitleWords.Contains(words[i]))
                words[i] = words[i].ToLower(ptBr);
        }

        return string.Join(' ', words);
    }
}

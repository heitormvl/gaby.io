using System.Text.Json.Serialization;

namespace Gaby.io.Services;

public class GoogleBooksSearchResponse
{
    [JsonPropertyName("items")]
    public List<GoogleBooksItem>? Items { get; set; }
}

public class GoogleBooksItem
{
    [JsonPropertyName("volumeInfo")]
    public GoogleBooksVolumeInfo? VolumeInfo { get; set; }
}

public class GoogleBooksVolumeInfo
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("authors")]
    public List<string>? Authors { get; set; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("publishedDate")]
    public string? PublishedDate { get; set; }

    [JsonPropertyName("pageCount")]
    public int? PageCount { get; set; }

    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }
}

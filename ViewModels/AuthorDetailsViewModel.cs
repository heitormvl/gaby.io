namespace Gaby.io.ViewModels;

public class AuthorDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? CountryName { get; set; }

    public List<AuthorBookSummary>? Books { get; set; } = new();
}

public class AuthorBookSummary
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

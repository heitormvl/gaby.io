namespace Gaby.io.ViewModels;

public class AuthorListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? CountryName { get; set; }
    public int BookCount { get; set; }
}

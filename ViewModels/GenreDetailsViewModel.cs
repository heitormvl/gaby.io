namespace Gaby.io.ViewModels;

public class GenreDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string>? Books { get; set; } = new();
}

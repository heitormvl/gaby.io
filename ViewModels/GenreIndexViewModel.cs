namespace Gaby.io.ViewModels;

public class GenreIndexViewModel
{
    public List<GenreListViewModel> Genres { get; set; } = new();
    public int TotalUniqueBooks { get; set; }
}

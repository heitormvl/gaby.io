namespace Gaby.io.ViewModels;

public class ReadingDetailsViewModel
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PagesRead { get; set; }
    public int TotalPages { get; set; }
    public int? Rating { get; set; }
}

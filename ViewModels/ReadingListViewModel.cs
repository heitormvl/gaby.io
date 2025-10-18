namespace Gaby.io.ViewModels;

public class ReadingListViewModel
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

namespace Gaby.io.ViewModels;

public class PagesByMonthViewModel
{
    public int Month { get; set; }           // 1–12
    public string MonthName { get; set; } = string.Empty; // "Janeiro", "Fevereiro", etc.
    public int TotalPages { get; set; }      // Soma de páginas lidas
}

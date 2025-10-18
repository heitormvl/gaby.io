namespace Gaby.io.ViewModels;

public class DashboardViewModel
{
    // Estatísticas gerais
    public int TotalUniqueBooks { get; set; }
    public int TotalPages { get; set; }
    public int PagesThisMonth { get; set; }
    public int MonthlyAverage { get; set; }

    // Dados para gráficos
    public List<PagesByMonthViewModel> PagesByMonth { get; set; } = new();
    public List<PagesByYearViewModel> PagesByYear { get; set; } = new();
    public List<GenresDistributionViewModel> GenresDistribution { get; set; } = new();
    public List<GenresByYearViewModel> GenresByYear { get; set; } = new();
}
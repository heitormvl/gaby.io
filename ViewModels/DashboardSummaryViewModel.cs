namespace Gaby.io.ViewModels;

public class DashboardSummaryViewModel
{
    public IEnumerable<PagesByMonthViewModel> PagesByMonth { get; set; } = new List<PagesByMonthViewModel>();
    public IEnumerable<PagesByYearViewModel> PagesByYear { get; set; } = new List<PagesByYearViewModel>();
    public IEnumerable<GenresDistributionViewModel> GenresDistribution { get; set; } = new List<GenresDistributionViewModel>();
    public IEnumerable<GenresByYearViewModel> GenresByYear { get; set; } = new List<GenresByYearViewModel>();
}

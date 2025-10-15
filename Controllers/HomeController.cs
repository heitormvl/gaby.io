using Microsoft.AspNetCore.Mvc;
using Gaby.io.ViewModels;

namespace Gaby.io.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Mock: páginas lidas por mês
        var pagesByMonth = new List<PagesByMonthViewModel>
        {
            new() { Month = 1, MonthName = "Janeiro", TotalPages = 520 },
            new() { Month = 2, MonthName = "Fevereiro", TotalPages = 380 },
            new() { Month = 3, MonthName = "Março", TotalPages = 610 },
            new() { Month = 4, MonthName = "Abril", TotalPages = 290 },
            new() { Month = 5, MonthName = "Maio", TotalPages = 730 },
        };

        // Mock: páginas lidas por ano
        var pagesByYear = new List<PagesByYearViewModel>
        {
            new() { Year = 2024, TotalPages = 4500 },
            new() { Year = 2025, TotalPages = 3200 },
        };

        // Mock: distribuição por gênero
        var genresDistribution = new List<GenresDistributionViewModel>
        {
            new() { GenreName = "Romance", BooksRead = 5, TotalPages = 2000 },
            new() { GenreName = "Ficção Científica", BooksRead = 3, TotalPages = 1300 },
            new() { GenreName = "Fantasia", BooksRead = 2, TotalPages = 900 },
            new() { GenreName = "Suspense", BooksRead = 4, TotalPages = 1000 },
        };

        // Mock: livros lidos por gênero e ano
        var genresByYear = new List<GenresByYearViewModel>
        {
            new() { Year = 2024, GenreName = "Romance", BooksRead = 3 },
            new() { Year = 2024, GenreName = "Fantasia", BooksRead = 2 },
            new() { Year = 2025, GenreName = "Romance", BooksRead = 2 },
            new() { Year = 2025, GenreName = "Ficção Científica", BooksRead = 3 },
            new() { Year = 2025, GenreName = "Suspense", BooksRead = 1 },
        };

        // Monta o summary geral
        var summary = new DashboardSummaryViewModel
        {
            PagesByMonth = pagesByMonth,
            PagesByYear = pagesByYear,
            GenresDistribution = genresDistribution,
            GenresByYear = genresByYear
        };

        return View(summary);
    }
}
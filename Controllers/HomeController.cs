using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gaby.io.Data;
using Gaby.io.ViewModels;
using System.Globalization;
using System.Security.Claims;

namespace Gaby.io.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Obtém o ID do usuário logado
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            // Se não houver usuário logado, retorna dashboard vazio
            return View(new DashboardViewModel());
        }

        // Busca todas as leituras do usuário com livros e gêneros
        var readings = await _context.Readings
            .Include(r => r.Book)
                .ThenInclude(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
            .Where(r => r.UserId == userId && r.Status == "Concluída")
            .ToListAsync();

        // 1. Páginas lidas por mês (ano atual)
        var currentYear = DateTime.Now.Year;
        var pagesByMonth = readings
            .Where(r => r.Year == currentYear && r.Month.HasValue)
            .GroupBy(r => r.Month!.Value)
            .Select(g => new PagesByMonthViewModel
            {
                Month = g.Key,
                MonthName = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetMonthName(g.Key),
                TotalPages = g.Sum(r => r.PagesRead)
            })
            .OrderBy(p => p.Month)
            .ToList();

        // 2. Páginas lidas por ano
        var pagesByYear = readings
            .Where(r => r.Year.HasValue)
            .GroupBy(r => r.Year!.Value)
            .Select(g => new PagesByYearViewModel
            {
                Year = g.Key,
                TotalPages = g.Sum(r => r.PagesRead)
            })
            .OrderBy(p => p.Year)
            .ToList();

        // 3. Distribuição por gênero (total de livros e páginas)
        var genresDistribution = readings
            .SelectMany(r => r.Book.BookGenres.Select(bg => new { Genre = bg.Genre.Name, Pages = r.PagesRead }))
            .GroupBy(x => x.Genre)
            .Select(g => new GenresDistributionViewModel
            {
                GenreName = g.Key,
                BooksRead = g.Count(),
                TotalPages = g.Sum(x => x.Pages)
            })
            .OrderByDescending(g => g.BooksRead)
            .ToList();

        // 4. Livros lidos por gênero e ano
        var genresByYear = readings
            .Where(r => r.Year.HasValue)
            .SelectMany(r => r.Book.BookGenres.Select(bg => new { Year = r.Year!.Value, Genre = bg.Genre.Name }))
            .GroupBy(x => new { x.Year, x.Genre })
            .Select(g => new GenresByYearViewModel
            {
                Year = g.Key.Year,
                GenreName = g.Key.Genre,
                BooksRead = g.Count()
            })
            .OrderBy(g => g.Year)
            .ThenBy(g => g.GenreName)
            .ToList();

        // Cálculos para estatísticas gerais
        var totalUniqueBooks = readings.Select(r => r.BookId).Distinct().Count();
        var totalPages = readings.Sum(r => r.PagesRead);
        var pagesThisMonth = pagesByMonth.LastOrDefault()?.TotalPages ?? 0;
        var monthlyAverage = pagesByMonth.Any() ? (int)pagesByMonth.Average(p => p.TotalPages) : 0;

        // Monta o dashboard
        var dashboard = new DashboardViewModel
        {
            TotalUniqueBooks = totalUniqueBooks,
            TotalPages = totalPages,
            PagesThisMonth = pagesThisMonth,
            MonthlyAverage = monthlyAverage,
            PagesByMonth = pagesByMonth,
            PagesByYear = pagesByYear,
            GenresDistribution = genresDistribution,
            GenresByYear = genresByYear
        };

        return View(dashboard);
    }
}
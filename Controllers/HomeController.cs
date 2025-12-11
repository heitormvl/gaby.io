using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gaby.io.Data;
using Gaby.io.ViewModels;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Gaby.io.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
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

        // Distribui as páginas proporcionalmente entre os meses
        var distributedReadings = DistributeReadingsByMonth(readings);

        // 1. Páginas lidas por mês (ano atual)
        var currentYear = DateTime.Now.Year;
        var pagesByMonth = distributedReadings
            .Where(dr => dr.Year == currentYear)
            .GroupBy(dr => dr.Month)
            .Select(g => new PagesByMonthViewModel
            {
                Month = g.Key,
                MonthName = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetMonthName(g.Key),
                TotalPages = g.Sum(dr => dr.Pages)
            })
            .OrderBy(p => p.Month)
            .ToList();

        // 2. Páginas lidas por ano
        var pagesByYear = distributedReadings
            .GroupBy(dr => dr.Year)
            .Select(g => new PagesByYearViewModel
            {
                Year = g.Key,
                TotalPages = g.Sum(dr => dr.Pages)
            })
            .OrderBy(p => p.Year)
            .ToList();

        // 3. Distribuição por gênero (total de livros e páginas distribuídas)
        var genresDistribution = distributedReadings
            .SelectMany(dr => dr.Book.BookGenres.Select(bg => new { Genre = bg.Genre.Name, Pages = dr.Pages }))
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
        var genresByYear = distributedReadings
            .SelectMany(dr => dr.Book.BookGenres.Select(bg => new { Year = dr.Year, Genre = bg.Genre.Name }))
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
        
        // Calcula páginas deste mês considerando a distribuição proporcional
        var currentMonth = DateTime.Now.Month;
        var pagesThisMonth = distributedReadings
            .Where(dr => dr.Year == currentYear && dr.Month == currentMonth)
            .Sum(dr => dr.Pages);
        
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

    /// <summary>
    /// Distribui as páginas lidas proporcionalmente entre os meses com base nas datas de início e término
    /// </summary>
    private List<DistributedReading> DistributeReadingsByMonth(List<Gaby.io.Models.ReadingModel> readings)
    {
        var distributedReadings = new List<DistributedReading>();

        foreach (var reading in readings)
        {
            // Se não tiver StartDate ou EndDate, usa a lógica antiga (considera apenas o mês de início)
            if (!reading.StartDate.HasValue || !reading.EndDate.HasValue)
            {
                if (reading.Year.HasValue && reading.Month.HasValue)
                {
                    distributedReadings.Add(new DistributedReading
                    {
                        ReadingId = reading.Id,
                        Year = reading.Year.Value,
                        Month = reading.Month.Value,
                        Pages = reading.PagesRead,
                        Book = reading.Book
                    });
                }
                continue;
            }

            var startDate = reading.StartDate.Value;
            var endDate = reading.EndDate.Value;

            // Se a leitura foi feita no mesmo mês, conta tudo nesse mês
            if (startDate.Year == endDate.Year && startDate.Month == endDate.Month)
            {
                distributedReadings.Add(new DistributedReading
                {
                    ReadingId = reading.Id,
                    Year = startDate.Year,
                    Month = startDate.Month,
                    Pages = reading.PagesRead,
                    Book = reading.Book
                });
                continue;
            }

            // Calcula a distribuição proporcional entre os meses
            var totalDays = (endDate - startDate).Days + 1; // +1 para incluir o dia final
            if (totalDays <= 0) totalDays = 1; // Proteção contra divisão por zero

            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);
            var endOfReading = endDate;

            while (currentDate <= endOfReading)
            {
                var firstDay = currentDate.Year == startDate.Year && currentDate.Month == startDate.Month 
                    ? startDate.Day 
                    : 1;
                
                var lastDay = currentDate.Year == endDate.Year && currentDate.Month == endDate.Month 
                    ? endDate.Day 
                    : DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                var daysInMonth = lastDay - firstDay + 1;
                var proportionalPages = (int)Math.Round((double)reading.PagesRead * daysInMonth / totalDays);

                if (proportionalPages > 0)
                {
                    distributedReadings.Add(new DistributedReading
                    {
                        ReadingId = reading.Id,
                        Year = currentDate.Year,
                        Month = currentDate.Month,
                        Pages = proportionalPages,
                        Book = reading.Book
                    });
                }

                // Avança para o próximo mês
                currentDate = currentDate.AddMonths(1);
            }
        }

        return distributedReadings;
    }

    /// <summary>
    /// Classe auxiliar para representar uma leitura distribuída por mês
    /// </summary>
    private class DistributedReading
    {
        public int ReadingId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Pages { get; set; }
        public Gaby.io.Models.BookModel Book { get; set; } = null!;
    }
}
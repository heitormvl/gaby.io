using Microsoft.AspNetCore.Mvc;
using Gaby.io.Data;
using Gaby.io.Models;
using Gaby.io.Services;
using Gaby.io.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Gaby.io.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly AppDbContext _context;
    private readonly IGoogleBooksService _googleBooksService;
    private readonly IWikidataService _wikidataService;

    public BookController(AppDbContext context, IGoogleBooksService googleBooksService, IWikidataService wikidataService)
    {
        _context = context;
        _googleBooksService = googleBooksService;
        _wikidataService = wikidataService;
    }

    public IActionResult Index()
    {
        var books = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
            .Select(b => new BookListViewModel
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author.Name,
                PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                GenreNames = b.BookGenres.Select(bg => bg.Genre.Name).ToList(),
                PageCount = b.PageCount
            })
            .OrderBy(b => b.Title)
            .ToList();

        return View(books);
    }

    public IActionResult Details(int id)
    {
        var bookModel = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
            .Include(b => b.Readings)
            .FirstOrDefault(b => b.Id == id);

        if (bookModel == null)
            return NotFound();

        // Calcular a nota média das avaliações
        var ratings = bookModel.Readings
            .Where(r => r.Rating.HasValue && r.Rating.Value > 0)
            .Select(r => r.Rating!.Value)
            .ToList();

        decimal? averageRating = null;
        int totalRatings = ratings.Count;

        if (totalRatings > 0)
        {
            averageRating = Math.Round((decimal)ratings.Average(), 1);
        }

        var book = new BookDetailsViewModel
        {
            Id = bookModel.Id,
            Title = bookModel.Title,
            AuthorId = bookModel.Author.Id,
            AuthorName = bookModel.Author.Name,
            PublisherName = bookModel.Publisher?.Name,
            GenreNames = bookModel.BookGenres.Select(bg => bg.Genre.Name).ToList(),
            PageCount = bookModel.PageCount,
            PublicationDate = bookModel.PublicationDate,
            AverageRating = averageRating,
            TotalRatings = totalRatings
        };

        return View(book);
    }

    public IActionResult Create()
    {
        var model = new BookFormViewModel
        {
            Authors = _context.Authors
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList(),
            Publishers = _context.Publishers
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToList(),
            Genres = _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Authors = _context.Authors
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList();
            model.Publishers = _context.Publishers
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToList();
            model.Genres = _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
                .ToList();
            return View(model);
        }

        var book = new BookModel
        {
            Title = model.Title,
            AuthorId = model.AuthorId,
            PublisherId = model.PublisherId,
            PageCount = model.PageCount,
            PublicationDate = model.PublicationDate
        };

        _context.Books.Add(book);
        _context.SaveChanges();

        // Adicionar os gêneros selecionados
        if (model.GenreIds != null && model.GenreIds.Any())
        {
            foreach (var genreId in model.GenreIds)
            {
                _context.BookGenres.Add(new BookGenreModel
                {
                    BookId = book.Id,
                    GenreId = genreId
                });
            }
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var bookModel = _context.Books
            .Include(b => b.BookGenres)
            .FirstOrDefault(b => b.Id == id);

        if (bookModel == null)
            return NotFound();

        var model = new BookFormViewModel
        {
            Id = bookModel.Id,
            Title = bookModel.Title,
            AuthorId = bookModel.AuthorId,
            PublisherId = bookModel.PublisherId,
            GenreIds = bookModel.BookGenres.Select(bg => bg.GenreId).ToList(),
            PageCount = bookModel.PageCount,
            PublicationDate = bookModel.PublicationDate,
            Authors = _context.Authors
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList(),
            Publishers = _context.Publishers
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToList(),
            Genres = _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id, BookFormViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            model.Authors = _context.Authors
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList();
            model.Publishers = _context.Publishers
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToList();
            model.Genres = _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
                .ToList();
            return View(model);
        }

        var bookToUpdate = _context.Books
            .Include(b => b.BookGenres)
            .FirstOrDefault(b => b.Id == id);

        if (bookToUpdate == null)
            return NotFound();

        bookToUpdate.Title = model.Title;
        bookToUpdate.AuthorId = model.AuthorId;
        bookToUpdate.PublisherId = model.PublisherId;
        bookToUpdate.PageCount = model.PageCount;
        bookToUpdate.PublicationDate = model.PublicationDate;

        // Remover gêneros antigos
        var existingGenres = bookToUpdate.BookGenres.ToList();
        foreach (var genre in existingGenres)
        {
            _context.BookGenres.Remove(genre);
        }

        // Adicionar novos gêneros
        if (model.GenreIds != null && model.GenreIds.Any())
        {
            foreach (var genreId in model.GenreIds)
            {
                _context.BookGenres.Add(new BookGenreModel
                {
                    BookId = bookToUpdate.Id,
                    GenreId = genreId
                });
            }
        }

        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var bookModel = _context.Books
            .Include(b => b.Author)
            .FirstOrDefault(b => b.Id == id);

        if (bookModel == null)
            return NotFound();

        var model = new BookDeleteViewModel
        {
            Id = bookModel.Id,
            Title = bookModel.Title,
            AuthorName = bookModel.Author.Name
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        var bookToDelete = _context.Books.Find(id);
        if (bookToDelete == null)
            return NotFound();

        _context.Books.Remove(bookToDelete);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAjax(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Json(new { success = false, errors });
        }

        var book = new BookModel
        {
            Title = model.Title,
            AuthorId = model.AuthorId,
            PublisherId = model.PublisherId,
            PageCount = model.PageCount,
            PublicationDate = model.PublicationDate
        };

        _context.Books.Add(book);
        _context.SaveChanges();

        // Adicionar os gêneros selecionados
        if (model.GenreIds != null && model.GenreIds.Any())
        {
            foreach (var genreId in model.GenreIds)
            {
                _context.BookGenres.Add(new BookGenreModel
                {
                    BookId = book.Id,
                    GenreId = genreId
                });
            }
            _context.SaveChanges();
        }

        return Json(new { success = true, id = book.Id, title = book.Title, pageCount = book.PageCount });
    }

    [HttpGet]
    public async Task<IActionResult> SearchGoogleBooks(string q, CancellationToken cancellationToken)
    {
        var results = await _googleBooksService.SearchAsync(q, cancellationToken);
        return Json(results);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResolveGoogleBookSelection(GoogleBookSearchResult selection)
    {
        if (string.IsNullOrWhiteSpace(selection.Title))
            return Json(new { success = false, errors = new[] { "Título inválido." } });

        var author = await FindOrCreateAuthorAsync(selection.AuthorName);
        var publisher = await FindOrCreatePublisherAsync(selection.PublisherName);
        var genre = await FindOrCreateGenreAsync(selection.SuggestedGenreName);

        return Json(new
        {
            success = true,
            title = selection.Title,
            pageCount = selection.PageCount,
            publicationDate = selection.PublicationDate?.ToString("yyyy-MM-dd"),
            authorId = author.Id,
            authorName = author.Name,
            publisherId = publisher?.Id,
            publisherName = publisher?.Name,
            genreId = genre?.Id,
            genreName = genre?.Name
        });
    }

    private async Task<AuthorModel> FindOrCreateAuthorAsync(string? name)
    {
        var authorName = string.IsNullOrWhiteSpace(name) ? "Autor desconhecido" : name.Trim();

        var existing = await _context.Authors
            .FirstOrDefaultAsync(a => a.Name.ToLower() == authorName.ToLower());
        if (existing != null)
            return existing;

        char gender = 'N';
        int? countryId = null;

        var enrichment = await _wikidataService.LookupAuthorAsync(authorName, HttpContext.RequestAborted);
        if (enrichment != null)
        {
            if (enrichment.Gender.HasValue)
                gender = enrichment.Gender.Value;

            if (!string.IsNullOrWhiteSpace(enrichment.CountryName))
            {
                var country = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == enrichment.CountryName.ToLower());
                countryId = country?.Id;
            }
        }

        var author = new AuthorModel
        {
            Name = authorName,
            Gender = gender,
            CountryId = countryId
        };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    private async Task<PublisherModel?> FindOrCreatePublisherAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var publisherName = name.Trim();
        var existing = await _context.Publishers
            .FirstOrDefaultAsync(p => p.Name.ToLower() == publisherName.ToLower());
        if (existing != null)
            return existing;

        var publisher = new PublisherModel { Name = publisherName };
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();
        return publisher;
    }

    private async Task<GenreModel?> FindOrCreateGenreAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var genreName = name.Trim();
        var existing = await _context.Genres
            .FirstOrDefaultAsync(g => g.Name.ToLower() == genreName.ToLower());
        if (existing != null)
            return existing;

        var genre = new GenreModel { Name = genreName };
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return genre;
    }
}
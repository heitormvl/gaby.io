using Microsoft.AspNetCore.Mvc;
using Gaby.io.Data;
using Gaby.io.Models;
using Gaby.io.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Gaby.io.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly AppDbContext _context;

    public BookController(AppDbContext context)
    {
        _context = context;
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
}
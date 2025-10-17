using Microsoft.AspNetCore.Mvc;
using Gaby.io.Data;
using Gaby.io.Models;
using Gaby.io.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gaby.io.Controllers;

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
            .Include(b => b.Genre)
            .Select(b => new BookListViewModel
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author.Name,
                PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                GenreName = b.Genre != null ? b.Genre.Name : null,
                PageCount = b.PageCount ?? 0
            })
            .ToList();

        return View(books);
    }

    public IActionResult Details(int id)
    {
        var bookModel = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.Genre)
            .FirstOrDefault(b => b.Id == id);

        if (bookModel == null)
            return NotFound();

        var book = new BookDetailsViewModel
        {
            Id = bookModel.Id,
            Title = bookModel.Title,
            AuthorName = bookModel.Author.Name,
            PublisherName = bookModel.Publisher?.Name,
            GenreName = bookModel.Genre?.Name,
            PageCount = bookModel.PageCount ?? 0,
            PublicationDate = null
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
            GenreId = model.GenreId,
            PageCount = model.PageCount
        };

        _context.Books.Add(book);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var bookModel = _context.Books.Find(id);
        if (bookModel == null)
            return NotFound();

        var model = new BookFormViewModel
        {
            Id = bookModel.Id,
            Title = bookModel.Title,
            AuthorId = bookModel.AuthorId,
            PublisherId = bookModel.PublisherId,
            GenreId = bookModel.GenreId,
            PageCount = bookModel.PageCount ?? 0,
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

        var bookToUpdate = _context.Books.Find(id);
        if (bookToUpdate == null)
            return NotFound();

        bookToUpdate.Title = model.Title;
        bookToUpdate.AuthorId = model.AuthorId;
        bookToUpdate.PublisherId = model.PublisherId;
        bookToUpdate.GenreId = model.GenreId;
        bookToUpdate.PageCount = model.PageCount;
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
}
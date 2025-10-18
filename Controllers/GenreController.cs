using Microsoft.AspNetCore.Mvc;
using Gaby.io.ViewModels;
using Gaby.io.Data;
using Gaby.io.Models;
using Microsoft.EntityFrameworkCore;

namespace Gaby.io.Controllers;

public class GenreController : Controller
{
    private readonly AppDbContext _context;

    public GenreController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var genres = _context.Genres
            .Include(g => g.BookGenres)
            .Select(g => new GenreListViewModel
            {
                Id = g.Id,
                Name = g.Name,
                BookCount = g.BookGenres.Count
            })
            .ToList();

        var viewModel = new GenreIndexViewModel
        {
            Genres = genres,
            TotalUniqueBooks = _context.Books.Count()
        };

        return View(viewModel);
    }

    public IActionResult Details(int id)
    {
        var genreModel = _context.Genres
            .Include(g => g.BookGenres)
                .ThenInclude(bg => bg.Book)
            .FirstOrDefault(g => g.Id == id);

        if (genreModel == null)
            return NotFound();

        var genre = new GenreDetailsViewModel
        {
            Id = genreModel.Id,
            Name = genreModel.Name,
            Books = genreModel.BookGenres.Select(bg => bg.Book.Title).ToList()
        };

        return View(genre);
    }

    public IActionResult Create()
    {
        return View(new GenreFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(GenreFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var genre = new GenreModel
        {
            Name = model.Name
        };

        _context.Genres.Add(genre);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var genreModel = _context.Genres.Find(id);
        if (genreModel == null)
            return NotFound();

        var model = new GenreFormViewModel
        {
            Id = genreModel.Id,
            Name = genreModel.Name
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, GenreFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var genreToUpdate = _context.Genres.Find(id);
        if (genreToUpdate == null)
            return NotFound();

        genreToUpdate.Name = model.Name;
        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    public IActionResult Delete(int id)
    {
        var genreModel = _context.Genres
            .Include(g => g.BookGenres)
            .FirstOrDefault(g => g.Id == id);

        if (genreModel == null)
            return NotFound();

        var model = new GenreDeleteViewModel
        {
            Id = genreModel.Id,
            Name = genreModel.Name
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var genreToDelete = _context.Genres.Find(id);
        if (genreToDelete == null)
            return NotFound();

        _context.Genres.Remove(genreToDelete);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}

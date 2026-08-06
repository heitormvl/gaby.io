using Microsoft.AspNetCore.Mvc;
using Gaby.io.ViewModels;
using Gaby.io.Data;
using Gaby.io.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Gaby.io.Controllers;

[Authorize]
public class AuthorController : Controller
{
    private readonly AppDbContext _context;

    public AuthorController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var authors = _context.Authors
            .Include(a => a.Country)
            .Include(a => a.Books)
            .Select(a => new AuthorListViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Gender = a.Gender == 'M' ? "Masculino" : a.Gender == 'F' ? "Feminino" : "Não-binário",
                CountryName = a.Country != null ? a.Country.Name : null,
                BookCount = a.Books.Count
            })
            .OrderBy(a => a.Name)
            .ToList();

        return View(authors);
    }

    public IActionResult Details(int id)
    {
        var authorModel = _context.Authors
            .Include(a => a.Country)
            .Include(a => a.Books)
            .FirstOrDefault(a => a.Id == id);

        if (authorModel == null)
            return NotFound();

        var author = new AuthorDetailsViewModel
        {
            Id = authorModel.Id,
            Name = authorModel.Name,
            Gender = authorModel.Gender == 'M' ? "Masculino" : authorModel.Gender == 'F' ? "Feminino" : "Não-binário",
            CountryName = authorModel.Country?.Name,
            Books = authorModel.Books
                .Select(b => new AuthorBookSummary { Id = b.Id, Title = b.Title })
                .ToList()
        };

        return View(author);
    }

    public IActionResult Create()
    {
        var model = new AuthorFormViewModel
        {
            Countries = _context.Countries
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AuthorFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Countries = _context.Countries
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();
            return View(model);
        }

        var author = new AuthorModel
        {
            Name = model.Name,
            Gender = char.ToUpper(model.Gender),
            CountryId = model.CountryId
        };

        _context.Authors.Add(author);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var authorModel = _context.Authors.Find(id);
        if (authorModel == null)
            return NotFound();

        var model = new AuthorFormViewModel
        {
            Id = authorModel.Id,
            Name = authorModel.Name,
            Gender = authorModel.Gender,
            CountryId = authorModel.CountryId,
            Countries = _context.Countries
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id, AuthorFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Countries = _context.Countries
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();
            return View(model);
        }

        var authorToUpdate = _context.Authors.Find(id);
        if (authorToUpdate == null)
            return NotFound();

        authorToUpdate.Name = model.Name;
        authorToUpdate.Gender = char.ToUpper(model.Gender);
        authorToUpdate.CountryId = model.CountryId;
        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var authorModel = _context.Authors
            .Include(a => a.Country)
            .Include(a => a.Books)
            .FirstOrDefault(a => a.Id == id);

        if (authorModel == null)
            return NotFound();

        var model = new AuthorDeleteViewModel
        {
            Id = authorModel.Id,
            Name = authorModel.Name,
            Gender = authorModel.Gender == 'M' ? "Masculino" : authorModel.Gender == 'F' ? "Feminino" : "Não-binário",
            CountryName = authorModel.Country?.Name,
            BookCount = authorModel.Books.Count
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        var authorToDelete = _context.Authors.Find(id);
        if (authorToDelete == null)
            return NotFound();

        _context.Authors.Remove(authorToDelete);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAjax(AuthorFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        var author = new AuthorModel
        {
            Name = model.Name,
            Gender = char.ToUpper(model.Gender),
            CountryId = model.CountryId
        };

        _context.Authors.Add(author);
        _context.SaveChanges();

        return Json(new { success = true, id = author.Id, name = author.Name });
    }

    [HttpGet]
    public IActionResult GetAuthors()
    {
        var authors = _context.Authors
            .OrderBy(a => a.Name)
            .Select(a => new { id = a.Id, name = a.Name })
            .ToList();

        return Json(authors);
    }
}
using Microsoft.AspNetCore.Mvc;
using Gaby.io.ViewModels;
using Gaby.io.Data;
using Gaby.io.Models;
using Microsoft.EntityFrameworkCore;

namespace Gaby.io.Controllers;

public class PublisherController : Controller
{
    private readonly AppDbContext _context;

    public PublisherController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var publishers = _context.Publishers
            .Include(p => p.Books)
            .Select(p => new PublisherListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                BookCount = p.Books.Count
            })
            .ToList();

        return View(publishers);
    }

    public IActionResult Details(int id)
    {
        var publisherModel = _context.Publishers
            .Include(p => p.Books)
            .FirstOrDefault(p => p.Id == id);

        if (publisherModel == null)
            return NotFound();

        var publisher = new PublisherDetailsViewModel
        {
            Id = publisherModel.Id,
            Name = publisherModel.Name,
            Books = publisherModel.Books.Select(b => b.Title).ToList()
        };

        return View(publisher);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PublisherFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var publisher = new PublisherModel
        {
            Name = model.Name
        };

        _context.Publishers.Add(publisher);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var publisherModel = _context.Publishers.Find(id);
        if (publisherModel == null)
            return NotFound();

        var model = new PublisherFormViewModel
        {
            Id = publisherModel.Id,
            Name = publisherModel.Name
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, PublisherFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var publisherToUpdate = _context.Publishers.Find(id);
        if (publisherToUpdate == null)
            return NotFound();

        publisherToUpdate.Name = model.Name;
        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    public IActionResult Delete(int id)
    {
        var publisherModel = _context.Publishers
            .Include(p => p.Books)
            .FirstOrDefault(p => p.Id == id);

        if (publisherModel == null)
            return NotFound();

        var model = new PublisherDeleteViewModel
        {
            Id = publisherModel.Id,
            Name = publisherModel.Name,
            BookCount = publisherModel.Books.Count
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var publisherToDelete = _context.Publishers.Find(id);
        if (publisherToDelete == null)
            return NotFound();

        _context.Publishers.Remove(publisherToDelete);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAjax(PublisherFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        var publisher = new PublisherModel
        {
            Name = model.Name
        };

        _context.Publishers.Add(publisher);
        _context.SaveChanges();

        return Json(new { success = true, id = publisher.Id, name = publisher.Name });
    }
}

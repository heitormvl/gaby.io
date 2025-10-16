using Microsoft.AspNetCore.Mvc;
using Gaby.io.ViewModels;
using Gaby.io.Data;
using Gaby.io.Models;
using Microsoft.EntityFrameworkCore;

namespace Gaby.io.Controllers;

public class CountryController : Controller
{
    private readonly AppDbContext _context;

    public CountryController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var countries = _context.Countries
            .Include(c => c.Authors)
            .Select(c => new CountryListViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                AuthorCount = c.Authors.Count
            })
            .ToList();

        return View(countries);
    }

    public IActionResult Details(int id)
    {
        var countryModel = _context.Countries
            .Include(c => c.Authors)
            .FirstOrDefault(c => c.Id == id);
        if (countryModel == null)
            return NotFound();

        var country = new CountryDetailsViewModel
        {
            Id = countryModel.Id,
            Name = countryModel.Name,
            Code = countryModel.Code,
            Authors = countryModel.Authors.Select(a => a.Name).ToList()
        };

        return View(country);
    }

    public IActionResult Create()
    {
    return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CountryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var country = new CountryModel
        {
            Name = model.Name,
            Code = model.Code
        };
        _context.Countries.Add(country);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var countryModel = _context.Countries.Find(id);
        if (countryModel == null)
            return NotFound();

        var model = new CountryFormViewModel
        {
            Id = countryModel.Id,
            Name = countryModel.Name,
            Code = countryModel.Code
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, CountryFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var countryToUpdate = _context.Countries.Find(id);
        if (countryToUpdate == null)
            return NotFound();

        countryToUpdate.Name = model.Name;
        countryToUpdate.Code = model.Code;
        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    public IActionResult Delete(int id)
    {
        var countryModel = _context.Countries.Find(id);
        if (countryModel == null)
            return NotFound();

        var model = new CountryDeleteViewModel
        {
            Id = countryModel.Id,
            Name = countryModel.Name,
            Code = countryModel.Code
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var countryToDelete = _context.Countries.Find(id);
        if (countryToDelete == null)
            return NotFound();

        _context.Countries.Remove(countryToDelete);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
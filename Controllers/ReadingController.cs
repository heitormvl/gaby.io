using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gaby.io.Data;
using Gaby.io.Models;
using Gaby.io.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace Gaby.io.Controllers;

public class ReadingController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<UserModel> _userManager;

    public ReadingController(AppDbContext context, UserManager<UserModel> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var readings = await _context.Readings
            .Where(r => r.UserId == userId)
            .Include(r => r.Book)
            .Select(r => new ReadingListViewModel
            {
                Id = r.Id,
                BookTitle = r.Book.Title,
                StartDate = new DateTime(r.Year ?? DateTime.Now.Year, r.Month ?? 1, 1),
                EndDate = null,
                Status = r.Year.HasValue ? "Concluída" : "Em progresso"
            })
            .ToListAsync();

        return View(readings);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var readingModel = await _context.Readings
            .Where(r => r.UserId == userId && r.Id == id)
            .Include(r => r.Book)
            .FirstOrDefaultAsync();

        if (readingModel == null)
            return NotFound();

        var reading = new ReadingDetailsViewModel
        {
            Id = readingModel.Id,
            BookTitle = readingModel.Book.Title,
            StartDate = new DateTime(readingModel.Year ?? DateTime.Now.Year, readingModel.Month ?? 1, 1),
            EndDate = null,
            Status = readingModel.Year.HasValue ? "Concluída" : "Em progresso",
            PagesRead = 0,
            TotalPages = readingModel.Book.PageCount ?? 0
        };

        return View(reading);
    }

    public async Task<IActionResult> Create()
    {
        var model = new ReadingFormViewModel
        {
            StartDate = DateTime.Now,
            Books = await _context.Books
                .OrderBy(b => b.Title)
                .Select(b => new SelectListItem 
                { 
                    Value = b.Id.ToString(), 
                    Text = b.Title 
                })
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReadingFormViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            model.Books = await _context.Books
                .OrderBy(b => b.Title)
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Title })
                .ToListAsync();
            return View(model);
        }

        var reading = new ReadingModel
        {
            BookId = model.BookId,
            UserId = userId,
            Year = model.StartDate.Year,
            Month = model.StartDate.Month,
            Rating = 0
        };

        _context.Readings.Add(reading);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var readingModel = await _context.Readings
            .Where(r => r.UserId == userId && r.Id == id)
            .FirstOrDefaultAsync();

        if (readingModel == null)
            return NotFound();

        var model = new ReadingFormViewModel
        {
            Id = readingModel.Id,
            BookId = readingModel.BookId,
            StartDate = new DateTime(readingModel.Year ?? DateTime.Now.Year, readingModel.Month ?? 1, 1),
            EndDate = null,
            Status = readingModel.Year.HasValue ? "Concluída" : "Em progresso",
            PagesRead = 0,
            Books = await _context.Books
                .OrderBy(b => b.Title)
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Title })
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReadingFormViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            model.Books = await _context.Books
                .OrderBy(b => b.Title)
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Title })
                .ToListAsync();
            return View(model);
        }

        var readingToUpdate = await _context.Readings
            .Where(r => r.UserId == userId && r.Id == id)
            .FirstOrDefaultAsync();

        if (readingToUpdate == null)
            return NotFound();

        readingToUpdate.BookId = model.BookId;
        readingToUpdate.Year = model.StartDate.Year;
        readingToUpdate.Month = model.StartDate.Month;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var readingModel = await _context.Readings
            .Where(r => r.UserId == userId && r.Id == id)
            .Include(r => r.Book)
            .FirstOrDefaultAsync();

        if (readingModel == null)
            return NotFound();

        var model = new ReadingDeleteViewModel
        {
            Id = readingModel.Id,
            BookTitle = readingModel.Book.Title,
            Status = readingModel.Year.HasValue ? "Concluída" : "Em progresso"
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var readingToDelete = await _context.Readings
            .Where(r => r.UserId == userId && r.Id == id)
            .FirstOrDefaultAsync();

        if (readingToDelete == null)
            return NotFound();

        _context.Readings.Remove(readingToDelete);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}

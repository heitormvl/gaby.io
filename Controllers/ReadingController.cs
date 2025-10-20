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
                StartDate = r.StartDate ?? new DateTime(r.Year ?? DateTime.Now.Year, r.Month ?? 1, 1),
                EndDate = r.EndDate,
                Status = r.Status
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
            StartDate = readingModel.StartDate ?? new DateTime(readingModel.Year ?? DateTime.Now.Year, readingModel.Month ?? 1, 1),
            EndDate = readingModel.EndDate,
            Status = readingModel.Status,
            PagesRead = readingModel.PagesRead,
            TotalPages = readingModel.Book.PageCount,
            Rating = readingModel.Rating
        };

        return View(reading);
    }

    public async Task<IActionResult> Create()
    {
        var model = new ReadingFormViewModel
        {
            StartDate = DateTime.Now,
            AvailableBooks = await _context.Books
                .OrderBy(b => b.Title)
                .Select(b => new BookSelectViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    PageCount = b.PageCount
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
            model.AvailableBooks = await _context.Books
                .OrderBy(b => b.Title)
                .Select(b => new BookSelectViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    PageCount = b.PageCount
                })
                .ToListAsync();

            return View(model);
        }

        var reading = new ReadingModel
        {
            BookId = model.BookId,
            UserId = userId,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Status = model.Status,
            PagesRead = model.PagesRead,
            Year = model.StartDate.Year,
            Month = model.StartDate.Month,
            Rating = model.Rating
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
            StartDate = readingModel.StartDate ?? new DateTime(readingModel.Year ?? DateTime.Now.Year, readingModel.Month ?? 1, 1),
            EndDate = readingModel.EndDate,
            Status = readingModel.Status,
            PagesRead = readingModel.PagesRead,
            Rating = readingModel.Rating,
            AvailableBooks = await _context.Books
                .OrderBy(b => b.Title)
                .Select(b => new BookSelectViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    PageCount = b.PageCount
                })
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
            model.AvailableBooks = await _context.Books
                .OrderBy(b => b.Title)
                .Select(b => new BookSelectViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    PageCount = b.PageCount
                })
                .ToListAsync();

            return View(model);
        }

        var readingToUpdate = await _context.Readings
            .Where(r => r.UserId == userId && r.Id == id)
            .FirstOrDefaultAsync();

        if (readingToUpdate == null)
            return NotFound();

        readingToUpdate.BookId = model.BookId;
        readingToUpdate.StartDate = model.StartDate;
        readingToUpdate.EndDate = model.EndDate;
        readingToUpdate.Status = model.Status;
        readingToUpdate.PagesRead = model.PagesRead;
        readingToUpdate.Rating = model.Rating;
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
            Status = readingModel.Status
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

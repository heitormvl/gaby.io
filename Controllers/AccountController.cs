using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Gaby.io.Data;
using Gaby.io.Models;
using Gaby.io.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Gaby.io.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<UserModel> _userManager;
    private readonly SignInManager<UserModel> _signInManager;

    public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    // GET: Account/Register
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new UserModel
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.Name
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (string.Equals(user.Email, RoleSeed.OwnerAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // GET: Account/EditDisplayName
    [Authorize]
    public async Task<IActionResult> EditDisplayName()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        var model = new EditDisplayNameViewModel
        {
            DisplayName = user.DisplayName
        };

        return View(model);
    }

    // POST: Account/EditDisplayName
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> EditDisplayName(EditDisplayNameViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        user.DisplayName = model.DisplayName;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [AllowAnonymous]
    // GET: Account/Login
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [AllowAnonymous]
    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToLocal(returnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Conta bloqueada. Tente novamente mais tarde.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
        }

        return View(model);
    }

    // GET: Account/Index (Profile)
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        var model = new AccountViewModel
        {
            Name = user.DisplayName,
            Email = user.Email ?? string.Empty,
            RegisteredAt = user.UserName != null ? DateTime.Now : DateTime.Now
        };

        return View(model);
    }

    // POST: Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}

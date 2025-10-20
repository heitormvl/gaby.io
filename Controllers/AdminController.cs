using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Gaby.io.Models;
using Gaby.io.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Gaby.io.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<UserModel> _userManager;
    private readonly RoleManager<IdentityRole<string>> _roleManager;

    public AdminController(UserManager<UserModel> userManager, RoleManager<IdentityRole<string>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        var userViewModels = new List<UserManagementViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userViewModels.Add(new UserManagementViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                IsAdmin = roles.Contains("Admin")
            });
        }

        return View(userViewModels);
    }

    // POST: Admin/ToggleAdmin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "Usuário não encontrado.";
            return RedirectToAction(nameof(Users));
        }

        // Não permitir que o usuário remova o próprio papel de admin
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Id == userId)
        {
            TempData["Error"] = "Você não pode remover o papel de administrador de si mesmo.";
            return RedirectToAction(nameof(Users));
        }

        // Verificar se o papel Admin existe, se não, criar
        var adminRole = await _roleManager.FindByNameAsync("Admin");
        if (adminRole == null)
        {
            adminRole = new IdentityRole<string>("Admin");
            await _roleManager.CreateAsync(adminRole);
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (isAdmin)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                TempData["Success"] = $"Papel de administrador removido de {user.DisplayName}.";
            }
            else
            {
                TempData["Error"] = "Erro ao remover papel de administrador.";
            }
        }
        else
        {
            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                TempData["Success"] = $"{user.DisplayName} agora é um administrador.";
            }
            else
            {
                TempData["Error"] = "Erro ao adicionar papel de administrador.";
            }
        }

        return RedirectToAction(nameof(Users));
    }

    // GET: Admin/Index
    public IActionResult Index()
    {
        return View();
    }

    // GET: Admin/AccessDenied
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}

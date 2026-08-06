using Microsoft.AspNetCore.Identity;
using Gaby.io.Models;

namespace Gaby.io.Data;

public static class RoleSeed
{
    // Dono do projeto: sempre deve ter a role Admin, seja o usuário já existente
    // (garantido aqui na inicialização) ou recém-registrado (garantido no AccountController).
    public const string OwnerAdminEmail = "heitormvl12@gmail.com";

    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<UserModel>>();

        // Criar role Admin se não existir
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<string>("Admin") { Id = Guid.NewGuid().ToString() });
        }

        // Garantir que o dono do projeto seja Admin, caso a conta já exista
        var ownerUser = await userManager.FindByEmailAsync(OwnerAdminEmail);
        if (ownerUser != null && !await userManager.IsInRoleAsync(ownerUser, "Admin"))
        {
            await userManager.AddToRoleAsync(ownerUser, "Admin");
        }
    }
}

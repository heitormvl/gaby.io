using Microsoft.AspNetCore.Identity;
using Gaby.io.Models;

namespace Gaby.io.Data;

public static class RoleSeed
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<UserModel>>();

        // Criar role Admin se não existir
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<string>("Admin"));
        }

        // Opcional: Criar um usuário admin padrão
        // Descomente as linhas abaixo para criar um admin automaticamente
        /*
        var adminEmail = "admin@gaby.io";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Administrador",
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        */
    }
}

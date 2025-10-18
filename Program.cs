using Gaby.io.Data;
using Gaby.io.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));

// Identity (usuários e autenticação)
builder.Services
    .AddIdentity<UserModel, IdentityRole<string>>(options =>
    {
        // Políticas de senha mais simples (ajuste conforme necessário)
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Custom ClaimsPrincipalFactory para adicionar DisplayName aos claims
builder.Services.AddScoped<IUserClaimsPrincipalFactory<UserModel>, Gaby.io.Factories.UserClaimsPrincipalFactory>();

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

// Build da aplicação
var app = builder.Build();

// Pipeline de middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity middleware
app.UseAuthentication();
app.UseAuthorization();

// Rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

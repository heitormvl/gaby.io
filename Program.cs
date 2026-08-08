using Gaby.io.Data;
using Gaby.io.Models;
using Gaby.io.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
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
    .AddRoles<IdentityRole<string>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Custom ClaimsPrincipalFactory para adicionar DisplayName aos claims
builder.Services.AddScoped<IUserClaimsPrincipalFactory<UserModel>, Gaby.io.Factories.UserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Admin/AccessDenied";
});

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

// Integração com a API do Google Books (busca de metadados para preenchimento automático)
builder.Services.Configure<GoogleBooksOptions>(builder.Configuration.GetSection("GoogleBooks"));
builder.Services.AddHttpClient<IGoogleBooksService, GoogleBooksService>(client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/books/v1/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Enriquecimento de autores (gênero e país) via Wikidata, ao criar um autor a partir do Google Books
builder.Services.AddHttpClient<IWikidataService, WikidataService>(client =>
{
    client.BaseAddress = new Uri("https://www.wikidata.org/");
    client.Timeout = TimeSpan.FromSeconds(8);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("gaby.io/1.0 (personal library app; https://github.com/heitormvl/gaby.io)");
});

// Envio de e-mails (recuperação de senha) via Resend
builder.Services.Configure<ResendOptions>(builder.Configuration.GetSection("Resend"));
builder.Services.AddHttpClient<IEmailSender, ResendEmailSender>(client =>
{
    client.BaseAddress = new Uri("https://api.resend.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Build da aplicação
var app = builder.Build();

// Seed de roles e admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await Gaby.io.Data.RoleSeed.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao criar roles e admin inicial");
    }
}

// Pipeline de middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseDeveloperExceptionPage();
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

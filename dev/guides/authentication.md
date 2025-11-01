# 🔐 Sistema de Autenticação e Autorização

Documentação completa do sistema de autenticação baseado em **ASP.NET Core Identity**.

## 📋 Visão Geral

O **gaby.io** usa o **ASP.NET Core Identity** para:
- ✅ Registro e login de usuários
- ✅ Gerenciamento de senhas (hash, validação)
- ✅ Autenticação baseada em cookies
- ✅ Autorização baseada em roles (Admin, User)
- ✅ Claims personalizados (DisplayName)

## 🏗️ Arquitetura

### Componentes Principais

```
┌─────────────────────────────────────────────┐
│          AccountController                  │
│  • Register()  • Login()  • Logout()        │
└──────────────┬──────────────────────────────┘
               │
               ↓
┌─────────────────────────────────────────────┐
│         UserManager<UserModel>              │
│  • CreateAsync()  • SignInAsync()           │
└──────────────┬──────────────────────────────┘
               │
               ↓
┌─────────────────────────────────────────────┐
│            Identity System                  │
│  • Password Hashing  • Cookie Auth          │
│  • Role Management   • Claims               │
└─────────────────────────────────────────────┘
```

## 👤 UserModel

O modelo customizado de usuário:

```csharp
public class UserModel : IdentityUser<string>
{
    [Required, MaxLength(50)]
    public string DisplayName { get; set; } = string.Empty;

    public ICollection<ReadingModel> Readings { get; set; } 
        = new List<ReadingModel>();
}
```

### Campos do Identity

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | string | Identificador único (GUID) |
| `UserName` | string | Nome de usuário (email) |
| `Email` | string | Email do usuário |
| `PasswordHash` | string | Senha criptografada |
| `EmailConfirmed` | bool | Email verificado? |
| `DisplayName` | string | **Campo customizado** - Nome de exibição |

## 🔧 Configuração (Program.cs)

### 1. Adicionar Identity

```csharp
builder.Services
    .AddIdentity<UserModel, IdentityRole<string>>(options =>
    {
        // Políticas de senha
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4; // Mínimo 4 caracteres
        
        // Lockout (bloqueio por tentativas falhas)
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        
        // Confirmação de email (desabilitado)
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddRoles<IdentityRole<string>>() // Suporte a roles
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```

### 2. Custom ClaimsPrincipalFactory

Adiciona `DisplayName` aos claims do usuário:

```csharp
builder.Services.AddScoped<
    IUserClaimsPrincipalFactory<UserModel>, 
    UserClaimsPrincipalFactory
>();
```

**Implementação:**

```csharp
public class UserClaimsPrincipalFactory : 
    UserClaimsPrincipalFactory<UserModel, IdentityRole<string>>
{
    public override async Task<ClaimsPrincipal> CreateAsync(UserModel user)
    {
        var principal = await base.CreateAsync(user);
        var identity = (ClaimsIdentity)principal.Identity!;
        
        // Adiciona DisplayName como claim
        identity.AddClaim(new Claim("DisplayName", user.DisplayName));
        
        return principal;
    }
}
```

### 3. Configurar Cookies

```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Admin/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Cookie expira em 7 dias
    options.SlidingExpiration = true; // Renova cookie automaticamente
});
```

### 4. Middleware

```csharp
// Ordem é importante!
app.UseRouting();
app.UseAuthentication(); // ← Identifica o usuário
app.UseAuthorization();  // ← Verifica permissões
app.MapControllers();
```

## 🎭 AccountController

### Registro de Usuário

```csharp
[HttpPost]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var user = new UserModel
    {
        UserName = model.Email,
        Email = model.Email,
        DisplayName = model.DisplayName
    };

    var result = await _userManager.CreateAsync(user, model.Password);

    if (result.Succeeded)
    {
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }

    return View(model);
}
```

### Login

```csharp
[HttpPost]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var result = await _signInManager.PasswordSignInAsync(
        model.Email,
        model.Password,
        isPersistent: model.RememberMe,
        lockoutOnFailure: true // Bloqueia após 5 tentativas
    );

    if (result.Succeeded)
    {
        return RedirectToAction("Index", "Home");
    }

    if (result.IsLockedOut)
    {
        ModelState.AddModelError("", "Conta bloqueada. Tente novamente em 5 minutos.");
    }
    else
    {
        ModelState.AddModelError("", "Email ou senha inválidos.");
    }

    return View(model);
}
```

### Logout

```csharp
[HttpPost]
public async Task<IActionResult> Logout()
{
    await _signInManager.SignOutAsync();
    return RedirectToAction("Index", "Home");
}
```

## 🔐 Autorização

### Atributos de Autorização

```csharp
// Controller inteiro requer autenticação
[Authorize]
public class BookController : Controller
{
    // Todas as actions requerem login
}

// Action específica requer role
[Authorize(Roles = "Admin")]
public IActionResult Delete(int id)
{
    // Apenas admins podem acessar
}

// Permitir acesso anônimo
[AllowAnonymous]
public IActionResult Index()
{
    // Qualquer um pode acessar
}
```

### Autorização nas Views

```html
@using Microsoft.AspNetCore.Authorization
@inject IAuthorizationService AuthorizationService

@if (User.Identity?.IsAuthenticated == true)
{
    <p>Bem-vindo, @User.FindFirst("DisplayName")?.Value</p>
}

@if (User.IsInRole("Admin"))
{
    <a href="/Admin">Painel Administrativo</a>
}
```

### Autorização Programática

```csharp
public async Task<IActionResult> Delete(int id)
{
    var book = await _context.Books.FindAsync(id);
    
    // Apenas o criador ou admin pode deletar
    if (book.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier) 
        && !User.IsInRole("Admin"))
    {
        return Forbid(); // 403 Forbidden
    }
    
    // processar...
}
```

## 👥 Sistema de Roles

### Roles Disponíveis

| Role | Descrição | Criação |
|------|-----------|---------|
| **Admin** | Acesso total ao sistema | Via RoleSeed ou SQL |
| **User** | Usuário padrão | Atribuído automaticamente |

### RoleSeed

Cria automaticamente a role Admin ao iniciar a aplicação:

```csharp
public static class RoleSeed
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<string>>>();
        
        // Criar role Admin
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<string>("Admin"));
        }
        
        // Opcional: criar usuário admin padrão
        var userManager = services.GetRequiredService<UserManager<UserModel>>();
        var adminEmail = "admin@gaby.io";
        
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new UserModel
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Administrador"
            };
            
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
```

### Gerenciar Roles

```csharp
// Adicionar usuário a role
await _userManager.AddToRoleAsync(user, "Admin");

// Remover usuário de role
await _userManager.RemoveFromRoleAsync(user, "Admin");

// Verificar se usuário está em role
bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

// Obter roles do usuário
var roles = await _userManager.GetRolesAsync(user);
```

## 🎫 Claims Personalizados

### Adicionar Claims

```csharp
var claims = new List<Claim>
{
    new Claim("DisplayName", user.DisplayName),
    new Claim("ProfilePicture", user.ProfilePictureUrl),
    new Claim("MemberSince", user.CreatedAt.ToString("yyyy-MM-dd"))
};

await _userManager.AddClaimsAsync(user, claims);
```

### Ler Claims

```csharp
// No controller
var displayName = User.FindFirstValue("DisplayName");
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

// Na view
@User.FindFirst("DisplayName")?.Value
```

## 🔒 Segurança

### Password Hashing

O Identity usa **PBKDF2** com sal aleatório:

```csharp
// Senha: "senha123"
// Hash: AQAAAAIAAYagAAAAEJ3... (longo e aleatório)
```

### Proteção CSRF

Sempre use tokens anti-forgery em formulários POST:

```html
<form method="post">
    @Html.AntiForgeryToken()
    <!-- campos -->
</form>
```

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
```

### Cookie Seguro

```csharp
options.Cookie.HttpOnly = true;  // Não acessível via JavaScript
options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Apenas HTTPS
options.Cookie.SameSite = SameSiteMode.Strict; // Proteção CSRF
```

## 📊 Fluxo de Autenticação

```
1. Usuário acessa /Account/Login
   ↓
2. Preenche email e senha
   ↓
3. POST para /Account/Login
   ↓
4. SignInManager valida credenciais
   ↓
5. Se válido: cria cookie de autenticação
   ↓
6. Redirect para página protegida
   ↓
7. Middleware lê cookie e autentica usuário
   ↓
8. Controller verifica [Authorize]
   ↓
9. Se autorizado: executa action
```

## 🧪 Testando Autenticação

### Criar Usuário de Teste

```bash
# Via interface web
1. Acesse /Account/Register
2. Preencha: DisplayName, Email, Password
3. Clique em "Registrar"

# Via SQL (desenvolvimento)
INSERT INTO AspNetUsers (Id, UserName, Email, DisplayName, PasswordHash)
VALUES (NEWID(), 'test@test.com', 'test@test.com', 'Test User', '...')
```

### Promover a Admin

```sql
-- Via script SQL (veja create-first-admin.sql)
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'test@test.com' AND r.Name = 'Admin'
```

## 📚 Próximos Passos

- [Gerenciamento de usuários (Admin)](../admin-management.md)
- [Dashboard e estatísticas](dashboard.md)
- [Sistema de avaliações](../rating-system.md)

---

**Referência:** [ASP.NET Core Identity Documentation](https://learn.microsoft.com/aspnet/core/security/authentication/identity)

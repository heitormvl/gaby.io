# 🎨 Padrões e Convenções

Este documento descreve os padrões arquiteturais, convenções de código e boas práticas utilizadas no **gaby.io**.

## 🏛️ Padrão Arquitetural: MVC

O projeto segue o padrão **Model-View-Controller (MVC)**:

```
┌─────────────────────────────────────────┐
│              Browser (User)             │
└─────────────────┬───────────────────────┘
                  │ HTTP Request
                  ↓
┌─────────────────────────────────────────┐
│          Controller (Logic)             │
│  • Recebe requisição                    │
│  • Processa dados                       │
│  • Retorna View                         │
└──────────┬────────────────┬─────────────┘
           │                │
           ↓                ↓
    ┌──────────┐      ┌──────────┐
    │  Model   │◄────►│   View   │
    │  (Data)  │      │  (UI)    │
    └──────────┘      └──────────┘
```

### Responsabilidades

| Camada         | Responsabilidade                     | Exemplo                            |
| -------------- | ------------------------------------ | ---------------------------------- |
| **Model**      | Representa dados e lógica de negócio | `BookModel`, `UserModel`           |
| **View**       | Renderiza interface do usuário       | `Index.cshtml`, `Details.cshtml`   |
| **Controller** | Orquestra Model e View               | `BookController`, `HomeController` |

## 🧩 Padrões Implementados

### 1. ViewModel Pattern

**Problema:** Models de entidade não são ideais para Views (expõem dados sensíveis, falta validações específicas)

**Solução:** ViewModels como camada intermediária

```csharp
// ❌ Não faça isso
public IActionResult Create()
{
    return View(new BookModel()); // Expõe toda a entidade
}

// ✅ Faça isso
public IActionResult Create()
{
    var viewModel = new BookFormViewModel
    {
        Authors = _context.Authors.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(),
            Text = a.Name
        }).ToList()
    };
    return View(viewModel);
}
```

### 2. Repository Pattern (Não Implementado)

**Decisão:** Optamos por **não** usar Repository Pattern porque:
- ✅ EF Core já abstrai o acesso a dados
- ✅ DbContext funciona como Unit of Work
- ✅ Menos camadas = menos complexidade para projeto pequeno/médio

```csharp
// Acesso direto ao DbContext
public async Task<IActionResult> Index()
{
    var books = await _context.Books
        .Include(b => b.Author)
        .ToListAsync();
    return View(books);
}
```

**Quando usar Repository:**
- Projetos grandes com múltiplas fontes de dados
- Necessidade de trocar ORM facilmente
- Lógica complexa de acesso a dados

### 3. Dependency Injection (DI)

Todo o projeto usa **Injeção de Dependência**:

```csharp
// Program.cs - Registro de serviços
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserClaimsPrincipalFactory<UserModel>, 
    UserClaimsPrincipalFactory>();

// Controller - Injeção via construtor
public class BookController : Controller
{
    private readonly AppDbContext _context;
    
    public BookController(AppDbContext context)
    {
        _context = context;
    }
}
```

### 4. Factory Pattern

**UserClaimsPrincipalFactory**: Customiza os claims do Identity

```csharp
public class UserClaimsPrincipalFactory : 
    UserClaimsPrincipalFactory<UserModel, IdentityRole<string>>
{
    public override async Task<ClaimsPrincipal> CreateAsync(UserModel user)
    {
        var principal = await base.CreateAsync(user);
        var identity = (ClaimsIdentity)principal.Identity!;
        
        // Adiciona DisplayName aos claims
        identity.AddClaim(new Claim("DisplayName", user.DisplayName));
        
        return principal;
    }
}
```

### 5. ViewComponent Pattern

Componentes reutilizáveis para encapsular lógica de UI:

```csharp
public class UserDisplayNameViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var displayName = User.FindFirst("DisplayName")?.Value 
            ?? "Usuário";
        return Content(displayName);
    }
}
```

Uso:
```html
<p>Bem-vindo, @await Component.InvokeAsync("UserDisplayName")</p>
```

## 📝 Convenções de Nomenclatura

### Classes

| Tipo          | Convenção                   | Exemplo                                   |
| ------------- | --------------------------- | ----------------------------------------- |
| Model         | `{Entity}Model`             | `BookModel`, `UserModel`                  |
| ViewModel     | `{Entity}{Action}ViewModel` | `BookFormViewModel`, `DashboardViewModel` |
| Controller    | `{Entity}Controller`        | `BookController`, `AdminController`       |
| ViewComponent | `{Name}ViewComponent`       | `UserDisplayNameViewComponent`            |

### Métodos e Variáveis

```csharp
// PascalCase para métodos públicos
public async Task<IActionResult> GetBookDetails(int id)

// camelCase para variáveis locais
var bookId = 123;
var authorName = "George Orwell";

// _camelCase para campos privados
private readonly AppDbContext _context;
```

### Rotas

```csharp
// Padrão: /{Controller}/{Action}/{id?}
/Books/Index         → BookController.Index()
/Books/Details/5     → BookController.Details(5)
/Books/Create        → BookController.Create() [GET]
/Books/Create        → BookController.Create(model) [POST]
```

## 🔒 Convenções de Segurança

### 1. Autorização

```csharp
// Controller inteiro requer autenticação
[Authorize]
public class BookController : Controller { }

// Action específica requer role Admin
[Authorize(Roles = "Admin")]
public IActionResult Delete(int id) { }

// Action pública (sobrescreve [Authorize] do controller)
[AllowAnonymous]
public IActionResult Index() { }
```

### 2. Anti-Forgery Tokens

```html
<!-- Sempre use em formulários POST -->
<form method="post">
    @Html.AntiForgeryToken()
    <!-- campos do formulário -->
</form>
```

```csharp
// Valide no controller
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(BookFormViewModel model)
```

### 3. Validação de Input

```csharp
// Model com Data Annotations
public class BookFormViewModel
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [MaxLength(200)]
    public string Title { get; set; }
    
    [Range(1, 10000, ErrorMessage = "Páginas devem estar entre 1 e 10000")]
    public int PageCount { get; set; }
}

// Validação no controller
[HttpPost]
public async Task<IActionResult> Create(BookFormViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }
    // processar...
}
```

## 🗄️ Convenções de Banco de Dados

### Relacionamentos e OnDelete

| Relação           | OnDelete   | Justificativa                        |
| ----------------- | ---------- | ------------------------------------ |
| User → Reading    | `Restrict` | Preservar histórico de leituras      |
| Book → Reading    | `Cascade`  | Leituras dependem de livros          |
| Author → Book     | `Restrict` | Impedir exclusão de autor com livros |
| Country → Author  | `SetNull`  | Autores podem ficar sem país         |
| Publisher → Book  | `SetNull`  | Livros podem ficar sem editora       |
| Genre ↔ BookGenre | `Cascade`  | Remover associações automaticamente  |

### Índices

```csharp
// Índice único composto
modelBuilder.Entity<BookModel>()
    .HasIndex(b => new { b.Title, b.AuthorId })
    .IsUnique();

// Índice para performance em consultas
modelBuilder.Entity<ReadingModel>()
    .HasIndex(r => new { r.UserId, r.BookId, r.Year, r.Month });
```

## 🎯 Boas Práticas

### 1. Async/Await

```csharp
// ✅ Use async para operações de I/O
public async Task<IActionResult> Index()
{
    var books = await _context.Books.ToListAsync();
    return View(books);
}

// ❌ Evite bloquear threads
public IActionResult Index()
{
    var books = _context.Books.ToList(); // Síncrono!
    return View(books);
}
```

### 2. Include para Eager Loading

```csharp
// ✅ Carregue relacionamentos necessários
var books = await _context.Books
    .Include(b => b.Author)
    .Include(b => b.Publisher)
    .Include(b => b.BookGenres)
        .ThenInclude(bg => bg.Genre)
    .ToListAsync();

// ❌ Evite N+1 queries
var books = await _context.Books.ToListAsync();
foreach (var book in books)
{
    var author = book.Author.Name; // Query adicional!
}
```

### 3. TempData para Mensagens

```csharp
// Controller
TempData["Success"] = "Livro criado com sucesso!";
TempData["Error"] = "Erro ao criar livro.";
return RedirectToAction(nameof(Index));

// View
@if (TempData["Success"] != null)
{
    <div class="alert alert-success">@TempData["Success"]</div>
}
```

### 4. Tratamento de Erros

```csharp
public async Task<IActionResult> Delete(int id)
{
    var book = await _context.Books.FindAsync(id);
    
    if (book == null)
    {
        return NotFound();
    }
    
    try
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Livro excluído com sucesso!";
    }
    catch (DbUpdateException)
    {
        TempData["Error"] = "Não foi possível excluir o livro.";
    }
    
    return RedirectToAction(nameof(Index));
}
```

## 📊 Padrões de Views

### Layout Compartilhado

Todas as views herdam de `_Layout.cshtml`:

```html
@{
    Layout = "_Layout";
    ViewData["Title"] = "Meus Livros";
}

<!-- Conteúdo da página -->
```

### Partial Views

```html
<!-- Reutilizar blocos de HTML -->
<partial name="_BookCard" model="book" />
```

### View Components

```html
<!-- Lógica + UI encapsulada -->
@await Component.InvokeAsync("UserDisplayName")
```

## 🧪 Convenções de Teste (Futuro)

Estrutura sugerida para testes:

```
gaby.io.Tests/
├── Controllers/
│   ├── BookControllerTests.cs
│   └── AdminControllerTests.cs
├── Models/
│   └── ValidationTests.cs
└── Integration/
    └── AuthenticationTests.cs
```

## 📚 Referências

- [ASP.NET Core MVC Best Practices](https://learn.microsoft.com/aspnet/core/mvc/)
- [Entity Framework Core Patterns](https://learn.microsoft.com/ef/core/)
- [C# Coding Conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/)

---

**Próximos Passos:**
- [Veja a estrutura do projeto](project-structure.md)
- [Entenda os controllers](../reference/controllers.md)
- [Aprenda sobre os models](../models.md)

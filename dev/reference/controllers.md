# 📖 Referência de Controllers

Documentação completa de todos os controllers do **gaby.io**.

## 📋 Visão Geral

O projeto possui **9 controllers principais**:

| Controller | Descrição | Autorização |
|------------|-----------|-------------|
| `HomeController` | Dashboard e página inicial | Pública |
| `AccountController` | Autenticação (Login, Register) | Pública |
| `BookController` | CRUD de livros | Requer login |
| `AuthorController` | CRUD de autores | Requer login |
| `PublisherController` | CRUD de editoras | Requer login |
| `GenreController` | CRUD de gêneros | Requer login |
| `CountryController` | CRUD de países | Requer login |
| `ReadingController` | CRUD de leituras | Requer login |
| `AdminController` | Painel administrativo | Apenas Admin |

---

## 🏠 HomeController

**Namespace:** `Gaby.io.Controllers`  
**Autorização:** `[AllowAnonymous]` (parcial)

### Actions

#### `Index()` - Dashboard

```csharp
[AllowAnonymous]
public async Task<IActionResult> Index()
```

**Descrição:** Renderiza o dashboard com estatísticas de leitura

**Retorno:** 
- View com `DashboardViewModel`
- Dashboard vazio se usuário não logado

**Query:**
```csharp
var readings = await _context.Readings
    .Include(r => r.Book)
        .ThenInclude(b => b.BookGenres)
        .ThenInclude(bg => bg.Genre)
    .Where(r => r.UserId == userId && r.Status == "Concluída")
    .ToListAsync();
```

**ViewModel:** `DashboardViewModel`

---

## 🔐 AccountController

**Namespace:** `Gaby.io.Controllers`  
**Autorização:** Mista

### Actions

#### `Register()` - GET

```csharp
[AllowAnonymous]
public IActionResult Register()
```

**Descrição:** Exibe formulário de registro

**Retorno:** View com `RegisterViewModel` vazio

#### `Register(RegisterViewModel)` - POST

```csharp
[HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel model)
```

**Validações:**
- Email obrigatório e válido
- Senha mínimo 4 caracteres
- DisplayName obrigatório (max 50 caracteres)

**Fluxo:**
1. Valida ModelState
2. Cria UserModel
3. UserManager.CreateAsync()
4. SignInManager.SignInAsync()
5. Redirect para Home/Index

**Erros Possíveis:**
- Email já cadastrado
- Senha muito fraca

#### `Login()` - GET

```csharp
[AllowAnonymous]
public IActionResult Login()
```

**Descrição:** Exibe formulário de login

#### `Login(LoginViewModel)` - POST

```csharp
[HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
```

**Validações:**
- Email obrigatório
- Senha obrigatória

**Fluxo:**
1. SignInManager.PasswordSignInAsync()
2. Se sucesso: Redirect para Home
3. Se falha: Exibe erro

**Lockout:** 5 tentativas falhas = bloqueio de 5 minutos

#### `Logout()` - POST

```csharp
[HttpPost]
[Authorize]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
```

**Descrição:** Desautentica usuário

**Fluxo:**
1. SignInManager.SignOutAsync()
2. Redirect para Home

---

## 📚 BookController

**Namespace:** `Gaby.io.Controllers`  
**Autorização:** `[Authorize]`

### Actions

#### `Index()` - Listar Livros

```csharp
public async Task<IActionResult> Index()
```

**Query:**
```csharp
var books = await _context.Books
    .Include(b => b.Author)
    .Include(b => b.Publisher)
    .Include(b => b.BookGenres)
        .ThenInclude(bg => bg.Genre)
    .OrderBy(b => b.Title)
    .ToListAsync();
```

**Retorno:** View com `List<BookListViewModel>`

#### `Details(int id)` - Detalhes do Livro

```csharp
public async Task<IActionResult> Details(int id)
```

**Query:**
```csharp
var book = await _context.Books
    .Include(b => b.Author)
        .ThenInclude(a => a.Country)
    .Include(b => b.Publisher)
    .Include(b => b.BookGenres)
        .ThenInclude(bg => bg.Genre)
    .FirstOrDefaultAsync(b => b.Id == id);
```

**Cálculo de Rating:**
```csharp
var ratings = await _context.Readings
    .Where(r => r.BookId == id && r.Rating > 0)
    .Select(r => r.Rating)
    .ToListAsync();

AverageRating = ratings.Any() ? ratings.Average() : null;
TotalRatings = ratings.Count;
```

**Retorno:** 
- View com `BookDetailsViewModel`
- NotFound() se livro não existe

#### `Create()` - GET

```csharp
public async Task<IActionResult> Create()
```

**Descrição:** Exibe formulário de criação

**Prepara dados:**
```csharp
var viewModel = new BookFormViewModel
{
    Authors = await _context.Authors
        .Select(a => new SelectListItem { Value = a.Id, Text = a.Name })
        .ToListAsync(),
    Publishers = await _context.Publishers
        .Select(p => new SelectListItem { Value = p.Id, Text = p.Name })
        .ToListAsync(),
    Genres = await _context.Genres
        .Select(g => new GenreCheckboxViewModel 
        { 
            Id = g.Id, 
            Name = g.Name, 
            IsSelected = false 
        })
        .ToListAsync()
};
```

#### `Create(BookFormViewModel)` - POST

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(BookFormViewModel model)
```

**Validações:**
- Title obrigatório (max 200)
- AuthorId obrigatório
- PageCount entre 1 e 10000
- Pelo menos 1 gênero selecionado

**Fluxo:**
1. Valida ModelState
2. Cria BookModel
3. Adiciona ao contexto
4. Salva relacionamentos BookGenre
5. SaveChangesAsync()
6. Redirect para Index

**Verificação de duplicata:**
```csharp
var exists = await _context.Books
    .AnyAsync(b => b.Title == model.Title && b.AuthorId == model.AuthorId);

if (exists)
{
    ModelState.AddModelError("Title", "Este livro já está cadastrado.");
    return View(model);
}
```

#### `Edit(int id)` - GET

```csharp
public async Task<IActionResult> Edit(int id)
```

**Descrição:** Exibe formulário de edição

**Carrega dados:**
```csharp
var book = await _context.Books
    .Include(b => b.BookGenres)
    .FirstOrDefaultAsync(b => b.Id == id);
```

**Marca gêneros selecionados:**
```csharp
Genres = await _context.Genres
    .Select(g => new GenreCheckboxViewModel
    {
        Id = g.Id,
        Name = g.Name,
        IsSelected = book.BookGenres.Any(bg => bg.GenreId == g.Id)
    })
    .ToListAsync();
```

#### `Edit(int id, BookFormViewModel)` - POST

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, BookFormViewModel model)
```

**Fluxo:**
1. Busca livro no banco
2. Atualiza propriedades
3. Remove relacionamentos antigos (BookGenres)
4. Adiciona novos relacionamentos
5. SaveChangesAsync()

**Tratamento de concorrência:**
```csharp
catch (DbUpdateConcurrencyException)
{
    if (!BookExists(id))
        return NotFound();
    throw;
}
```

#### `Delete(int id)` - GET

```csharp
public async Task<IActionResult> Delete(int id)
```

**Descrição:** Exibe confirmação de exclusão

**Retorno:** View com `BookDeleteViewModel`

#### `DeleteConfirmed(int id)` - POST

```csharp
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
```

**Fluxo:**
1. Busca livro
2. Remove do contexto
3. SaveChangesAsync()
4. TempData["Success"]
5. Redirect para Index

**Tratamento de erro:**
```csharp
catch (DbUpdateException)
{
    TempData["Error"] = "Não é possível excluir este livro.";
    return RedirectToAction(nameof(Index));
}
```

---

## ✍️ AuthorController

**Namespace:** `Gaby.io.Controllers`  
**Autorização:** `[Authorize]`

### Actions

#### `Index()` - Listar Autores

```csharp
public async Task<IActionResult> Index()
```

**Query:**
```csharp
var authors = await _context.Authors
    .Include(a => a.Country)
    .Include(a => a.Books)
    .OrderBy(a => a.Name)
    .ToListAsync();
```

**Retorno:** View com `List<AuthorListViewModel>`

**Calcula:**
- Nome do autor
- País
- Total de livros escritos

#### `Details(int id)` - Detalhes do Autor

```csharp
public async Task<IActionResult> Details(int id)
```

**Query:**
```csharp
var author = await _context.Authors
    .Include(a => a.Country)
    .Include(a => a.Books)
        .ThenInclude(b => b.Publisher)
    .FirstOrDefaultAsync(a => a.Id == id);
```

**Retorno:** View com `AuthorDetailsViewModel`

**Exibe:**
- Dados do autor
- Lista de todos os livros do autor

#### `Create()` - GET

```csharp
public async Task<IActionResult> Create()
```

**Prepara:**
```csharp
Countries = await _context.Countries
    .OrderBy(c => c.Name)
    .Select(c => new SelectListItem 
    { 
        Value = c.Id, 
        Text = c.Name 
    })
    .ToListAsync()
```

#### `Create(AuthorFormViewModel)` - POST

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(AuthorFormViewModel model)
```

**Validações:**
- Name obrigatório (max 100)
- Gender obrigatório ('M' ou 'F')
- CountryId opcional

**Verifica duplicata:**
```csharp
var exists = await _context.Authors
    .AnyAsync(a => a.Name == model.Name);
```

#### `Edit(int id)` - GET/POST

Similar ao BookController

#### `Delete(int id)` - GET

Exibe confirmação com:
- Nome do autor
- Total de livros
- **Aviso:** se autor tem livros, não pode ser excluído (Restrict)

#### `DeleteConfirmed(int id)` - POST

```csharp
catch (DbUpdateException)
{
    TempData["Error"] = "Não é possível excluir este autor pois ele possui livros cadastrados.";
    return RedirectToAction(nameof(Index));
}
```

---

## 📖 ReadingController

**Namespace:** `Gaby.io.Controllers`  
**Autorização:** `[Authorize]`

### Actions

#### `Index()` - Listar Leituras

```csharp
public async Task<IActionResult> Index()
```

**Query:**
```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

var readings = await _context.Readings
    .Include(r => r.Book)
        .ThenInclude(b => b.Author)
    .Where(r => r.UserId == userId)
    .OrderByDescending(r => r.StartDate)
    .ToListAsync();
```

**Filtro:** Apenas leituras do usuário logado

#### `Details(int id)` - Detalhes da Leitura

```csharp
public async Task<IActionResult> Details(int id)
```

**Verificação de autorização:**
```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
if (reading.UserId != userId)
{
    return Forbid(); // 403
}
```

**Retorno:** View com `ReadingDetailsViewModel`

#### `Create()` - GET

```csharp
public async Task<IActionResult> Create()
```

**Prepara:**
```csharp
Books = await _context.Books
    .Include(b => b.Author)
    .Select(b => new SelectListItem
    {
        Value = b.Id,
        Text = $"{b.Title} - {b.Author.Name}"
    })
    .ToListAsync()
```

#### `Create(ReadingFormViewModel)` - POST

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(ReadingFormViewModel model)
```

**Validações:**
- BookId obrigatório
- StartDate obrigatória
- EndDate >= StartDate (se preenchida)
- Rating entre 0 e 5 (se preenchida)
- Status obrigatório
- PagesRead >= 0

**Cálculo automático:**
```csharp
// Se EndDate preenchida, calcula Year e Month
if (model.EndDate.HasValue)
{
    reading.Year = model.EndDate.Value.Year;
    reading.Month = model.EndDate.Value.Month;
}
```

**Adiciona UserId:**
```csharp
reading.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
```

#### `Edit(int id)` - GET/POST

Similar ao Create, mas verifica autorização:

```csharp
if (reading.UserId != userId)
{
    return Forbid();
}
```

#### `Delete(int id)` - GET/POST

Verifica autorização antes de excluir:

```csharp
if (reading.UserId != userId)
{
    return Forbid();
}
```

---

## 🔐 AdminController

**Namespace:** `Gaby.io.Controllers`  
**Autorização:** `[Authorize(Roles = "Admin")]`

### Actions

#### `Index()` - Dashboard Admin

```csharp
public IActionResult Index()
```

**Descrição:** Página inicial do painel administrativo

#### `Users()` - Listar Usuários

```csharp
public async Task<IActionResult> Users()
```

**Query:**
```csharp
var users = await _userManager.Users.ToListAsync();

foreach (var user in users)
{
    var roles = await _userManager.GetRolesAsync(user);
    // Mapeia para UserManagementViewModel
}
```

**Retorno:** View com `List<UserManagementViewModel>`

**Exibe:**
- DisplayName
- Email
- IsAdmin (booleano)
- Botões para conceder/remover Admin

#### `ToggleAdmin(string userId)` - POST

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ToggleAdmin(string userId)
```

**Validações:**
- Usuário existe?
- Não é o próprio admin tentando remover a si mesmo

**Fluxo:**
```csharp
var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

if (isAdmin)
{
    await _userManager.RemoveFromRoleAsync(user, "Admin");
    TempData["Success"] = "Papel removido.";
}
else
{
    await _userManager.AddToRoleAsync(user, "Admin");
    TempData["Success"] = "Papel concedido.";
}
```

#### `AccessDenied()` - Acesso Negado

```csharp
[AllowAnonymous]
public IActionResult AccessDenied()
```

**Descrição:** Exibe página de acesso negado (403)

---

## 📝 GenreController, PublisherController, CountryController

Seguem o mesmo padrão do `AuthorController`:

- **Index()**: Listar
- **Details(id)**: Detalhes
- **Create()**: Formulário GET/POST
- **Edit(id)**: Formulário GET/POST
- **Delete(id)**: Confirmação GET/POST

**Diferenças:**

- **GenreController**: Verifica duplicata de nome (índice único)
- **PublisherController**: Permite deletar apenas se não tiver livros
- **CountryController**: Seed automático, raramente precisa CRUD manual

---

## 🔄 Padrões Comuns

### Estrutura Típica de Action

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(FormViewModel model)
{
    // 1. Validar ModelState
    if (!ModelState.IsValid)
    {
        // Recarregar listas (dropdowns, etc)
        return View(model);
    }
    
    // 2. Verificar lógica de negócio
    if (await DuplicateExists(model))
    {
        ModelState.AddModelError("", "Já existe.");
        return View(model);
    }
    
    try
    {
        // 3. Criar entidade
        var entity = new Model { /* ... */ };
        
        // 4. Adicionar ao contexto
        _context.Add(entity);
        
        // 5. Salvar
        await _context.SaveChangesAsync();
        
        // 6. Mensagem de sucesso
        TempData["Success"] = "Criado com sucesso!";
        
        // 7. Redirect
        return RedirectToAction(nameof(Index));
    }
    catch (DbUpdateException)
    {
        // 8. Tratar erros
        TempData["Error"] = "Erro ao salvar.";
        return View(model);
    }
}
```

### Tratamento de Erros

```csharp
// NotFound (404)
if (entity == null)
    return NotFound();

// Forbidden (403)
if (entity.UserId != currentUserId)
    return Forbid();

// BadRequest (400)
if (!ModelState.IsValid)
    return BadRequest(ModelState);

// Redirect com mensagem
TempData["Error"] = "Erro!";
return RedirectToAction(nameof(Index));
```

## 📚 Próximos Passos

- [ViewModels Reference](viewmodels.md)
- [Rotas do Sistema](routes.md)
- [Guia de Autenticação](../guides/authentication.md)

---

**Convenção de Rotas:** `/{Controller}/{Action}/{id?}`

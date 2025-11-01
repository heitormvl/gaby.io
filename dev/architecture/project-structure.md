# 🏗️ Estrutura do Projeto

Visão geral da organização de pastas e arquivos do **gaby.io**.

## 📂 Estrutura de Diretórios

```
gaby.io/
├── 📁 Controllers/           # Controllers MVC (lógica de negócio)
│   ├── AccountController.cs     # Autenticação (Login, Register, Logout)
│   ├── AdminController.cs       # Painel administrativo
│   ├── AuthorController.cs      # CRUD de autores
│   ├── BookController.cs        # CRUD de livros
│   ├── CountryController.cs     # CRUD de países
│   ├── GenreController.cs       # CRUD de gêneros
│   ├── HomeController.cs        # Dashboard e página inicial
│   ├── PublisherController.cs   # CRUD de editoras
│   └── ReadingController.cs     # CRUD de leituras
│
├── 📁 Models/                # Modelos de entidade (mapeamento do banco)
│   ├── AuthorModel.cs           # Autor (Id, Name, Gender, CountryId)
│   ├── BookGenreModel.cs        # Relacionamento N:N (Book ↔ Genre)
│   ├── BookModel.cs             # Livro (Id, Title, AuthorId, PublisherId, etc.)
│   ├── CountryModel.cs          # País (Id, Name, Code)
│   ├── GenreModel.cs            # Gênero (Id, Name)
│   ├── PublisherModel.cs        # Editora (Id, Name)
│   ├── ReadingModel.cs          # Leitura (Id, BookId, UserId, Rating, etc.)
│   └── UserModel.cs             # Usuário (Identity + DisplayName)
│
├── 📁 ViewModels/            # ViewModels para comunicação Controller ↔ View
│   ├── AccountViewModel.cs      # Login/Register
│   ├── AuthorFormViewModel.cs   # Criar/Editar autor
│   ├── AuthorListViewModel.cs   # Listar autores
│   ├── BookFormViewModel.cs     # Criar/Editar livro
│   ├── BookDetailsViewModel.cs  # Detalhes do livro (+ AverageRating)
│   ├── DashboardViewModel.cs    # Dashboard com gráficos
│   ├── ReadingFormViewModel.cs  # Criar/Editar leitura (+ Rating)
│   ├── UserManagementViewModel.cs  # Gerenciar usuários (Admin)
│   └── ...
│
├── 📁 Views/                 # Views Razor (.cshtml)
│   ├── 📁 Account/              # Login, Register
│   ├── 📁 Admin/                # Painel administrativo
│   ├── 📁 Author/               # CRUD de autores
│   ├── 📁 Book/                 # CRUD de livros
│   ├── 📁 Country/              # CRUD de países
│   ├── 📁 Genre/                # CRUD de gêneros
│   ├── 📁 Home/                 # Dashboard (Index.cshtml)
│   ├── 📁 Publisher/            # CRUD de editoras
│   ├── 📁 Reading/              # CRUD de leituras
│   └── 📁 Shared/               # Layout, _ViewImports, _ValidationScripts
│
├── 📁 Data/                  # Contexto do EF Core e Seeds
│   ├── AppDbContext.cs          # DbContext principal (configuração do EF)
│   ├── CountrySeed.cs           # Seed inicial de países (extension method)
│   └── RoleSeed.cs              # Seed de roles e admin inicial
│
├── 📁 Factories/             # Factories personalizadas
│   └── UserClaimsPrincipalFactory.cs  # Adiciona DisplayName aos claims
│
├── 📁 ViewComponents/        # View Components reutilizáveis
│   └── UserDisplayNameViewComponent.cs  # Exibe nome do usuário logado
│
├── 📁 Migrations/            # Histórico de migrations do EF Core
│   ├── 20251015162725_Initial.cs
│   ├── 20251018171724_AddReadingDetailsFields.cs
│   ├── 20251018173714_AddMultipleGenresSupport.cs
│   ├── 20251019043404_SeedCountries.cs
│   ├── 20251020030731_ChangeRatingToInteger.cs
│   └── AppDbContextModelSnapshot.cs
│
├── 📁 wwwroot/               # Arquivos estáticos (CSS, JS, imagens)
│   ├── 📁 css/                  # Estilos personalizados
│   ├── 📁 js/                   # Scripts JavaScript
│   ├── 📁 lib/                  # Bibliotecas (Bootstrap, jQuery, etc.)
│   └── favicon.ico
│
├── 📁 Properties/            # Configurações do projeto
│   └── launchSettings.json      # Perfis de execução (IIS, Kestrel)
│
├── 📁 dev/                   # 📚 Documentação do projeto
│   ├── README.md                # Índice da documentação
│   ├── stack.md                 # Stack tecnológica
│   ├── models.md                # Documentação dos modelos
│   ├── admin-quick-start.md     # Criar primeiro admin
│   ├── admin-management.md      # Gerenciamento de usuários
│   ├── rating-system.md         # Sistema de avaliações
│   ├── create-first-admin.sql   # Script SQL
│   ├── 📁 getting-started/      # Guias de início
│   ├── 📁 architecture/         # Documentação arquitetural
│   ├── 📁 guides/               # Guias de funcionalidades
│   └── 📁 reference/            # Referências técnicas
│
├── 📄 Program.cs             # Ponto de entrada da aplicação (configuração)
├── 📄 appsettings.json       # Configurações gerais (connection string)
├── 📄 appsettings.Development.json  # Configurações de desenvolvimento
├── 📄 gaby.io.csproj         # Arquivo de projeto (.NET)
├── 📄 gaby.io.sln            # Solução do Visual Studio
└── 📄 README.md              # Documentação principal do repositório
```

## 🧩 Descrição dos Componentes

### 🎮 Controllers

Os **Controllers** são responsáveis por:
- Receber requisições HTTP
- Processar lógica de negócio
- Interagir com o banco de dados via EF Core
- Retornar Views ou Redirects

**Exemplo:**
```csharp
[Authorize]
public class BookController : Controller
{
    private readonly AppDbContext _context;
    
    public async Task<IActionResult> Index()
    {
        var books = await _context.Books
            .Include(b => b.Author)
            .ToListAsync();
        return View(books);
    }
}
```

### 🗄️ Models

Os **Models** representam as entidades do banco de dados:
- Mapeados pelo Entity Framework Core
- Contêm Data Annotations para validação
- Definem relacionamentos entre tabelas

**Exemplo:**
```csharp
public class BookModel
{
    [Key]
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string Title { get; set; }
    
    [ForeignKey(nameof(AuthorId))]
    public AuthorModel Author { get; set; }
}
```

### 📦 ViewModels

Os **ViewModels** são DTOs (Data Transfer Objects) para:
- Transferir dados entre Controller e View
- Evitar expor Models diretamente
- Adicionar validações específicas da View

**Exemplo:**
```csharp
public class BookFormViewModel
{
    [Required(ErrorMessage = "O título é obrigatório")]
    public string Title { get; set; }
    
    [Required]
    public int AuthorId { get; set; }
    
    // Lista de autores para dropdown
    public List<SelectListItem> Authors { get; set; }
}
```

### 🖼️ Views

As **Views** são templates Razor (.cshtml):
- Renderizam HTML dinâmico
- Utilizam sintaxe Razor (`@Model`, `@if`, `@foreach`)
- Herdam de `_Layout.cshtml`

**Exemplo:**
```html
@model BookDetailsViewModel

<h1>@Model.Title</h1>
<p>Autor: @Model.AuthorName</p>
<p>⭐ @Model.AverageRating (@Model.TotalRatings avaliações)</p>
```

### 🗃️ Data

- **AppDbContext.cs**: Configuração do Entity Framework Core
  - DbSets (tabelas)
  - Relacionamentos (OnDelete Cascade, Restrict, SetNull)
  - Índices e constraints
  
- **Seeds**: Dados iniciais (países, roles, admin)

### 📊 ViewComponents

Componentes reutilizáveis que encapsulam lógica e UI:
```csharp
public class UserDisplayNameViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var displayName = User.FindFirst("DisplayName")?.Value;
        return Content(displayName ?? "Usuário");
    }
}
```

Uso na View:
```html
@await Component.InvokeAsync("UserDisplayName")
```

## 🔄 Fluxo de Requisição

```
1. User → HTTP Request
   ↓
2. Router → Identifica Controller/Action
   ↓
3. Controller → Processa requisição
   ↓
4. AppDbContext → Consulta banco de dados (EF Core)
   ↓
5. ViewModel → Mapeia dados para View
   ↓
6. View (Razor) → Renderiza HTML
   ↓
7. Response → Envia HTML ao navegador
```

## 📜 Convenções do Projeto

### Nomenclatura

- **Models**: `{Entity}Model.cs` (ex: `BookModel.cs`)
- **ViewModels**: `{Entity}{Action}ViewModel.cs` (ex: `BookFormViewModel.cs`)
- **Controllers**: `{Entity}Controller.cs` (ex: `BookController.cs`)
- **Views**: `/Views/{Controller}/{Action}.cshtml`

### Padrões

- **Repository Pattern**: Não implementado (EF Core abstraído diretamente)
- **Dependency Injection**: Services registrados em `Program.cs`
- **Authorization**: Atributos `[Authorize]` e `[Authorize(Roles = "Admin")]`
- **Validation**: Data Annotations + ModelState

## 🔐 Segurança

- **Identity**: Autenticação e autorização
- **Anti-Forgery Tokens**: Proteção CSRF
- **Password Hashing**: Bcrypt (Identity padrão)
- **HTTPS**: Forçado em produção

## 📚 Próximos Passos

- [Entenda os padrões arquiteturais](patterns.md)
- [Veja a documentação dos Controllers](../reference/controllers.md)
- [Aprenda sobre o sistema de avaliações](../rating-system.md)

---

**Dica:** Use o Visual Studio Solution Explorer ou `tree /F` para visualizar a estrutura completa!

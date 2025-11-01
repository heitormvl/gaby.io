# 📊 Dashboard e Estatísticas

Documentação do sistema de dashboard com gráficos e visualizações de dados de leitura.

## 📋 Visão Geral

O **Dashboard** (`HomeController.Index`) apresenta estatísticas detalhadas das leituras do usuário:

- 📈 **Páginas por Mês** - Gráfico de barras mostrando páginas lidas mensalmente
- 📚 **Páginas por Ano** - Gráfico de linhas com evolução anual
- 🎨 **Distribuição por Gênero** - Gráfico de pizza com gêneros mais lidos
- 📊 **Livros por Gênero e Ano** - Gráfico de barras agrupadas

## 🏗️ Arquitetura

### Fluxo de Dados

```
User → HomeController.Index()
         ↓
    Query leituras do usuário (EF Core)
         ↓
    Processar dados e calcular estatísticas
         ↓
    Criar DashboardViewModel
         ↓
    Renderizar View com gráficos (Chart.js)
```

## 📦 DashboardViewModel

```csharp
public class DashboardViewModel
{
    // Estatísticas gerais
    public int TotalUniqueBooks { get; set; }
    public int TotalPages { get; set; }
    public int PagesThisMonth { get; set; }
    public int MonthlyAverage { get; set; }

    // Dados para gráficos
    public List<PagesByMonthViewModel> PagesByMonth { get; set; }
    public List<PagesByYearViewModel> PagesByYear { get; set; }
    public List<GenresDistributionViewModel> GenresDistribution { get; set; }
    public List<GenresByYearViewModel> GenresByYear { get; set; }
}
```

### ViewModels Auxiliares

```csharp
public class PagesByMonthViewModel
{
    public int Month { get; set; }        // 1-12
    public string MonthName { get; set; } // "Janeiro", "Fevereiro"...
    public int TotalPages { get; set; }
}

public class PagesByYearViewModel
{
    public int Year { get; set; }
    public int TotalPages { get; set; }
}

public class GenresDistributionViewModel
{
    public string GenreName { get; set; }
    public int BooksRead { get; set; }
    public int TotalPages { get; set; }
}

public class GenresByYearViewModel
{
    public int Year { get; set; }
    public string GenreName { get; set; }
    public int BooksRead { get; set; }
}
```

## 🎮 HomeController.Index()

### 1. Buscar Leituras

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

var readings = await _context.Readings
    .Include(r => r.Book)
        .ThenInclude(b => b.BookGenres)
        .ThenInclude(bg => bg.Genre)
    .Where(r => r.UserId == userId && r.Status == "Concluída")
    .ToListAsync();
```

**Filtros:**
- Apenas leituras do usuário logado
- Apenas leituras concluídas
- Inclui relacionamentos (Book → BookGenres → Genre)

### 2. Processar Páginas por Mês

```csharp
var currentYear = DateTime.Now.Year;

var pagesByMonth = readings
    .Where(r => r.Year == currentYear && r.Month.HasValue)
    .GroupBy(r => r.Month!.Value)
    .Select(g => new PagesByMonthViewModel
    {
        Month = g.Key,
        MonthName = CultureInfo
            .GetCultureInfo("pt-BR")
            .DateTimeFormat
            .GetMonthName(g.Key),
        TotalPages = g.Sum(r => r.PagesRead)
    })
    .OrderBy(p => p.Month)
    .ToList();
```

### 3. Processar Páginas por Ano

```csharp
var pagesByYear = readings
    .Where(r => r.Year.HasValue)
    .GroupBy(r => r.Year!.Value)
    .Select(g => new PagesByYearViewModel
    {
        Year = g.Key,
        TotalPages = g.Sum(r => r.PagesRead)
    })
    .OrderBy(p => p.Year)
    .ToList();
```

### 4. Distribuição por Gênero

```csharp
var genresDistribution = readings
    .SelectMany(r => r.Book.BookGenres.Select(bg => new 
    { 
        Genre = bg.Genre.Name, 
        Pages = r.PagesRead 
    }))
    .GroupBy(x => x.Genre)
    .Select(g => new GenresDistributionViewModel
    {
        GenreName = g.Key,
        BooksRead = g.Count(),
        TotalPages = g.Sum(x => x.Pages)
    })
    .OrderByDescending(g => g.BooksRead)
    .ToList();
```

**Nota:** Um livro pode ter múltiplos gêneros, então usamos `SelectMany` para "desdobrar" os gêneros.

### 5. Livros por Gênero e Ano

```csharp
var genresByYear = readings
    .Where(r => r.Year.HasValue)
    .SelectMany(r => r.Book.BookGenres.Select(bg => new 
    { 
        Year = r.Year!.Value, 
        Genre = bg.Genre.Name 
    }))
    .GroupBy(x => new { x.Year, x.Genre })
    .Select(g => new GenresByYearViewModel
    {
        Year = g.Key.Year,
        GenreName = g.Key.Genre,
        BooksRead = g.Count()
    })
    .OrderBy(g => g.Year)
    .ThenBy(g => g.GenreName)
    .ToList();
```

### 6. Estatísticas Gerais

```csharp
var totalUniqueBooks = readings
    .Select(r => r.BookId)
    .Distinct()
    .Count();

var totalPages = readings.Sum(r => r.PagesRead);

var pagesThisMonth = pagesByMonth
    .LastOrDefault()?.TotalPages ?? 0;

var monthlyAverage = pagesByMonth.Any() 
    ? (int)pagesByMonth.Average(p => p.TotalPages) 
    : 0;
```

## 🎨 Visualização (Views/Home/Index.cshtml)

### Cards de Estatísticas

```html
<div class="row mb-4">
    <div class="col-md-3">
        <div class="card text-center">
            <div class="card-body">
                <h5 class="card-title">Livros Lidos</h5>
                <p class="display-4">@Model.TotalUniqueBooks</p>
            </div>
        </div>
    </div>
    
    <div class="col-md-3">
        <div class="card text-center">
            <div class="card-body">
                <h5 class="card-title">Total de Páginas</h5>
                <p class="display-4">@Model.TotalPages.ToString("N0")</p>
            </div>
        </div>
    </div>
    
    <!-- Mais cards... -->
</div>
```

### Gráficos com Chart.js

**1. Páginas por Mês (Barras)**

```javascript
const pagesByMonthData = {
    labels: @Html.Raw(Json.Serialize(Model.PagesByMonth.Select(p => p.MonthName))),
    datasets: [{
        label: 'Páginas Lidas',
        data: @Html.Raw(Json.Serialize(Model.PagesByMonth.Select(p => p.TotalPages))),
        backgroundColor: 'rgba(54, 162, 235, 0.5)',
        borderColor: 'rgba(54, 162, 235, 1)',
        borderWidth: 2
    }]
};

new Chart(document.getElementById('pagesByMonthChart'), {
    type: 'bar',
    data: pagesByMonthData,
    options: {
        responsive: true,
        scales: {
            y: { beginAtZero: true }
        }
    }
});
```

**2. Páginas por Ano (Linhas)**

```javascript
const pagesByYearData = {
    labels: @Html.Raw(Json.Serialize(Model.PagesByYear.Select(p => p.Year))),
    datasets: [{
        label: 'Páginas Lidas',
        data: @Html.Raw(Json.Serialize(Model.PagesByYear.Select(p => p.TotalPages))),
        borderColor: 'rgba(75, 192, 192, 1)',
        backgroundColor: 'rgba(75, 192, 192, 0.2)',
        tension: 0.4,
        fill: true
    }]
};

new Chart(document.getElementById('pagesByYearChart'), {
    type: 'line',
    data: pagesByYearData
});
```

**3. Distribuição por Gênero (Pizza)**

```javascript
const genresData = {
    labels: @Html.Raw(Json.Serialize(Model.GenresDistribution.Select(g => g.GenreName))),
    datasets: [{
        data: @Html.Raw(Json.Serialize(Model.GenresDistribution.Select(g => g.BooksRead))),
        backgroundColor: [
            'rgba(255, 99, 132, 0.5)',
            'rgba(54, 162, 235, 0.5)',
            'rgba(255, 206, 86, 0.5)',
            'rgba(75, 192, 192, 0.5)',
            'rgba(153, 102, 255, 0.5)'
        ]
    }]
};

new Chart(document.getElementById('genresChart'), {
    type: 'pie',
    data: genresData,
    options: {
        plugins: {
            legend: { position: 'right' }
        }
    }
});
```

**4. Livros por Gênero e Ano (Barras Agrupadas)**

```javascript
// Agrupar dados por gênero
const genresByYear = @Html.Raw(Json.Serialize(Model.GenresByYear));
const years = [...new Set(genresByYear.map(g => g.year))];
const genres = [...new Set(genresByYear.map(g => g.genreName))];

const datasets = genres.map((genre, index) => ({
    label: genre,
    data: years.map(year => {
        const item = genresByYear.find(g => g.year === year && g.genreName === genre);
        return item ? item.booksRead : 0;
    }),
    backgroundColor: colors[index % colors.length]
}));

new Chart(document.getElementById('genresByYearChart'), {
    type: 'bar',
    data: {
        labels: years,
        datasets: datasets
    },
    options: {
        responsive: true,
        scales: {
            x: { stacked: false },
            y: { beginAtZero: true }
        }
    }
});
```

## 🎯 Casos de Uso

### Dashboard Vazio (Novo Usuário)

```csharp
if (string.IsNullOrEmpty(userId))
{
    return View(new DashboardViewModel());
}
```

Exibe mensagem:
```html
@if (Model.TotalUniqueBooks == 0)
{
    <div class="alert alert-info">
        Você ainda não tem leituras registradas. 
        <a href="/Reading/Create">Adicione sua primeira leitura!</a>
    </div>
}
```

### Múltiplos Gêneros por Livro

Um livro pode ter vários gêneros. O sistema conta cada gênero separadamente:

**Exemplo:**
- Livro "1984" tem gêneros: "Ficção", "Distopia"
- Dashboard conta 1 leitura para "Ficção" e 1 para "Distopia"

### Filtragem por Ano

O gráfico "Páginas por Mês" mostra apenas o **ano atual**:

```csharp
var currentYear = DateTime.Now.Year;
var pagesByMonth = readings
    .Where(r => r.Year == currentYear && r.Month.HasValue)
    // ...
```

Para ver anos anteriores, adicionar filtro na View.

## 📊 Performance

### Otimizações Aplicadas

1. **Eager Loading**: Carrega todos os relacionamentos de uma vez
   ```csharp
   .Include(r => r.Book)
       .ThenInclude(b => b.BookGenres)
       .ThenInclude(bg => bg.Genre)
   ```

2. **Filtragem no Banco**: Where antes de ToListAsync()
   ```csharp
   .Where(r => r.UserId == userId && r.Status == "Concluída")
   ```

3. **Projeção**: Select apenas campos necessários (evita trazer objetos inteiros)

### Melhorias Futuras

- [ ] Cache do dashboard (Redis ou Memory Cache)
- [ ] Paginação de dados históricos
- [ ] Lazy loading de gráficos (carregar sob demanda)
- [ ] API para atualização assíncrona

## 🧪 Testes

### Cenários de Teste

```csharp
// Teste: Dashboard vazio para novo usuário
[Fact]
public async Task Index_NewUser_ReturnsEmptyDashboard()
{
    // Arrange
    var controller = CreateController(userId: "new-user");
    
    // Act
    var result = await controller.Index();
    
    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    var model = Assert.IsType<DashboardViewModel>(viewResult.Model);
    Assert.Equal(0, model.TotalUniqueBooks);
}

// Teste: Cálculo correto de páginas por mês
[Fact]
public async Task Index_CalculatesPagesByMonth_Correctly()
{
    // Implementar...
}
```

## 📚 Próximos Passos

- [Sistema de leituras](readings.md)
- [Sistema de avaliações](../rating-system.md)
- [Autenticação e usuários](authentication.md)

---

**Bibliotecas usadas:**
- [Chart.js](https://www.chartjs.org/) - Gráficos interativos
- [Bootstrap 5](https://getbootstrap.com/) - Layout e cards

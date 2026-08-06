# ⚙️ Stack Tecnológica

Visão completa das tecnologias utilizadas no **gaby.io**.

## 🏗️ Arquitetura Geral

```
┌─────────────────────────────────────────────┐
│           Frontend (Razor Views)            │
│  Bootstrap 5 + Chart.js + Font Awesome      │
└──────────────────┬──────────────────────────┘
                   │ HTTP/HTTPS
                   ↓
┌─────────────────────────────────────────────┐
│        ASP.NET Core 8.0 MVC Backend         │
│  Controllers + ViewModels + Authorization   │
└──────────────────┬──────────────────────────┘
                   │ Entity Framework Core
                   ↓
┌─────────────────────────────────────────────┐
│           PostgreSQL Database                │
│  Identity Tables + Application Tables       │
└─────────────────────────────────────────────┘
```

## 🎨 Frontend

### Razor Views (.cshtml)
- **Versão:** ASP.NET Core 8.0
- **Engine:** Razor Pages
- **Sintaxe:** HTML + C# (`@Model`, `@if`, `@foreach`)

### CSS Framework
- **Bootstrap:** 5.3.2
  - Layout responsivo
  - Sistema de grid
  - Componentes (cards, modals, forms)
  - Temas customizados

### JavaScript
- **Chart.js:** 4.4.0
  - Gráficos de barras (páginas por mês)
  - Gráficos de linhas (páginas por ano)
  - Gráficos de pizza (distribuição por gênero)
  - Gráficos de barras agrupadas (gêneros por ano)

### Ícones
- **Font Awesome:** 6.5.1
  - Ícones vetoriais
  - Estrelas de avaliação
  - Ícones de ações (editar, excluir)

### Biblioteca Adicional
- **jQuery:** 3.7.1 (para validação de forms)

## 🔧 Backend

### Framework Principal
- **ASP.NET Core:** 8.0
- **Padrão:** MVC (Model-View-Controller)
- **Linguagem:** C# 12.0

### ORM (Mapeamento Objeto-Relacional)
- **Entity Framework Core:** 8.0.0
  - Code-First Migrations
  - LINQ Queries
  - Relacionamentos (1:N, N:N)
  - Eager Loading (Include/ThenInclude)

### Autenticação e Autorização
- **ASP.NET Core Identity:** 8.0.0
  - User Management
  - Role-based Authorization
  - Password Hashing (PBKDF2)
  - Cookie Authentication
  - Claims (DisplayName customizado)

### Dependências NuGet

```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.0" />
```

### Ferramentas CLI
- **dotnet-ef:** 8.0.0 (Entity Framework CLI)

## 🗄️ Banco de Dados

### SGBD
- **PostgreSQL:** 15+ (hospedado via Supabase)
- **Provider EF Core:** Npgsql.EntityFrameworkCore.PostgreSQL
- **Desenvolvimento local:** PostgreSQL via Docker

### Estrutura
- **Tabelas de Identity:**
  - AspNetUsers
  - AspNetRoles
  - AspNetUserRoles
  - AspNetUserClaims
  - AspNetRoleClaims
  - AspNetUserLogins
  - AspNetUserTokens

- **Tabelas da Aplicação:**
  - Countries (195 países)
  - Authors
  - Publishers
  - Genres
  - Books
  - BookGenres (N:N)
  - Readings

### Migrations
- **Total:** 7 migrations
- **Ferramentas:** EF Core Migrations
- **Comandos:**
  ```bash
  dotnet ef migrations add NomeDaMigration
  dotnet ef database update
  ```

## 🔐 Segurança

### Proteções Implementadas
- ✅ **HTTPS:** Forçado em produção
- ✅ **Anti-CSRF:** Tokens em todos os forms POST
- ✅ **Password Hashing:** PBKDF2 (Identity padrão)
- ✅ **SQL Injection:** Proteção via EF Core (parametrização)
- ✅ **XSS:** Proteção via Razor (encode automático)
- ✅ **Authorization:** Atributos `[Authorize]` e roles

### Configurações de Senha
```csharp
options.Password.RequiredLength = 4; // Desenvolvimento (aumentar em produção)
options.Password.RequireDigit = false;
options.Password.RequireUppercase = false;
options.Password.RequireNonAlphanumeric = false;
```

**⚠️ Nota:** Para produção, aumentar requisitos de senha!

## 🚀 Hospedagem e Deploy

### Ambientes Suportados

**Desenvolvimento:**
- Windows 10/11
- macOS
- Linux (Ubuntu, Fedora)

**Produção (sugestões):**
- Azure App Service
- AWS Elastic Beanstalk
- Heroku
- Docker Container
- IIS (Windows Server)

### Docker (Opcional)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["gaby.io.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gaby.io.dll"]
```

## 📊 Performance

### Otimizações Aplicadas
- **Eager Loading:** Include/ThenInclude para evitar N+1 queries
- **Async/Await:** Operações assíncronas em I/O
- **Índices no Banco:** 
  - Books: (Title, AuthorId) UNIQUE
  - BookGenres: (BookId, GenreId) UNIQUE
  - Readings: (UserId, BookId, Year, Month)
  - Countries: (Code) UNIQUE
  - Authors: (Name)

### Melhorias Futuras
- [ ] Cache (Redis ou Memory Cache)
- [ ] CDN para assets estáticos
- [ ] Minificação de CSS/JS
- [ ] Paginação em listas grandes
- [ ] Lazy Loading de imagens

## 🧪 Testes (Planejado)

### Frameworks Sugeridos
- **xUnit:** Testes unitários
- **Moq:** Mocking
- **FluentAssertions:** Assertions legíveis
- **Selenium:** Testes E2E

## 📦 Estrutura de Arquivos

```
gaby.io/
├── Controllers/          # Lógica de negócio
├── Models/              # Entidades do banco
├── ViewModels/          # DTOs para Views
├── Views/               # Templates Razor
├── Data/                # DbContext e Seeds
├── Factories/           # Factories customizadas
├── ViewComponents/      # Componentes reutilizáveis
├── Migrations/          # Histórico do banco
├── wwwroot/             # Assets estáticos
│   ├── css/
│   ├── js/
│   └── lib/            # Bootstrap, jQuery, Chart.js
├── Properties/          # launchSettings.json
├── dev/                 # 📚 Documentação
├── Program.cs           # Configuração da aplicação
├── appsettings.json     # Configurações gerais
└── gaby.io.csproj       # Projeto .NET
```

## 🔗 Links Úteis

### Documentação Oficial
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Bootstrap 5](https://getbootstrap.com/)
- [Chart.js](https://www.chartjs.org/)

### Ferramentas
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [VS Code](https://code.visualstudio.com/)
- [pgAdmin](https://www.pgadmin.org/)
- [Supabase](https://supabase.com/)

## 📌 Versões

| Tecnologia | Versão | Data de Release |
|------------|--------|-----------------|
| .NET SDK | 8.0.x | Nov 2023 |
| ASP.NET Core | 8.0.0 | Nov 2023 |
| EF Core | 8.0.0 | Nov 2023 |
| C# | 12.0 | Nov 2023 |
| Bootstrap | 5.3.2 | Set 2023 |
| Chart.js | 4.4.0 | Out 2023 |

---

**Última atualização:** 06/08/2026

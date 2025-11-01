# 📋 Histórico de Migrations

Documentação completa de todas as migrations do banco de dados do **gaby.io**.

## 📊 Visão Geral

O projeto possui **7 migrations** que refletem a evolução do banco de dados:

| # | Nome | Data | Descrição |
|---|------|------|-----------|
| 1 | Initial | 15/10/2025 | Estrutura inicial do banco |
| 2 | AddReadingDetailsFields | 18/10/2025 | Campos adicionais em Reading |
| 3 | UpdateBookModel | 18/10/2025 | Ajustes no modelo Book |
| 4 | AddMultipleGenresSupport | 18/10/2025 | Suporte a múltiplos gêneros |
| 5 | SeedCountries | 19/10/2025 | Seed de 195 países |
| 6 | AddUniqueGenreNameIndex | 19/10/2025 | Índice único para Genre.Name |
| 7 | ChangeRatingToInteger | 20/10/2025 | Altera Rating de decimal para int |

---

## 1️⃣ 20251015162725_Initial

**Data:** 15/10/2025 16:27:25  
**Descrição:** Migration inicial que cria toda a estrutura base do banco de dados

### Tabelas Criadas

#### AspNetUsers (Identity)
```sql
CREATE TABLE AspNetUsers (
    Id nvarchar(450) NOT NULL PRIMARY KEY,
    UserName nvarchar(256),
    Email nvarchar(256),
    PasswordHash nvarchar(max),
    DisplayName nvarchar(50) NOT NULL,
    -- Outros campos do Identity...
)
```

#### Countries
```sql
CREATE TABLE Countries (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(50) NOT NULL,
    Code nvarchar(3) NOT NULL
)
```

#### Authors
```sql
CREATE TABLE Authors (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    CountryId int NULL,
    Gender char(1) NOT NULL,
    CONSTRAINT FK_Author_Country FOREIGN KEY (CountryId) 
        REFERENCES Countries(Id) ON DELETE SET NULL
)
```

#### Publishers
```sql
CREATE TABLE Publishers (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL
)
```

#### Genres
```sql
CREATE TABLE Genres (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(50) NOT NULL
)
```

#### Books
```sql
CREATE TABLE Books (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Title nvarchar(200) NOT NULL,
    AuthorId int NOT NULL,
    PublisherId int NULL,
    PageCount int NOT NULL,
    PublicationDate date NULL,
    CONSTRAINT FK_Book_Author FOREIGN KEY (AuthorId) 
        REFERENCES Authors(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Book_Publisher FOREIGN KEY (PublisherId) 
        REFERENCES Publishers(Id) ON DELETE SET NULL
)
```

#### Readings
```sql
CREATE TABLE Readings (
    Id int IDENTITY(1,1) PRIMARY KEY,
    BookId int NOT NULL,
    UserId nvarchar(450) NOT NULL,
    Year int NULL,
    Month int NULL,
    Rating decimal(2,1) NULL, -- Depois muda para int
    CONSTRAINT FK_Reading_Book FOREIGN KEY (BookId) 
        REFERENCES Books(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Reading_User FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE RESTRICT
)
```

---

## 2️⃣ 20251018171724_AddReadingDetailsFields

**Data:** 18/10/2025 17:17:24  
**Descrição:** Adiciona campos detalhados para rastreamento de leituras

### Alterações

```sql
ALTER TABLE Readings ADD StartDate date NULL;
ALTER TABLE Readings ADD EndDate date NULL;
ALTER TABLE Readings ADD Status nvarchar(20) NOT NULL DEFAULT 'Em progresso';
ALTER TABLE Readings ADD PagesRead int NOT NULL DEFAULT 0;
```

### Novos Campos

| Campo | Tipo | Descrição | Validação |
|-------|------|-----------|-----------|
| StartDate | date | Data de início da leitura | Obrigatória |
| EndDate | date | Data de término da leitura | Opcional, >= StartDate |
| Status | nvarchar(20) | Status da leitura | "Em progresso", "Concluída", etc. |
| PagesRead | int | Número de páginas lidas | Range(0, 10000) |

---

## 3️⃣ 20251018172646_UpdateBookModel

**Data:** 18/10/2025 17:26:46  
**Descrição:** Ajustes e melhorias no modelo Book

### Alterações

- Ajustes de constraints e validações
- Preparação para relacionamento N:N com Genres

---

## 4️⃣ 20251018173714_AddMultipleGenresSupport

**Data:** 18/10/2025 17:37:14  
**Descrição:** Implementa suporte a múltiplos gêneros por livro (relacionamento N:N)

### Nova Tabela: BookGenres

```sql
CREATE TABLE BookGenres (
    Id int IDENTITY(1,1) PRIMARY KEY,
    BookId int NOT NULL,
    GenreId int NOT NULL,
    CONSTRAINT FK_BookGenre_Book FOREIGN KEY (BookId) 
        REFERENCES Books(Id) ON DELETE CASCADE,
    CONSTRAINT FK_BookGenre_Genre FOREIGN KEY (GenreId) 
        REFERENCES Genres(Id) ON DELETE CASCADE
)

CREATE UNIQUE INDEX IX_BookGenres_BookId_GenreId 
    ON BookGenres(BookId, GenreId);
```

### Relacionamento

```
Books (1) ←→ (N) BookGenres (N) ←→ (1) Genres
```

**Benefícios:**
- Um livro pode ter múltiplos gêneros
- Um gênero pode estar em múltiplos livros
- Evita duplicatas (índice único)

---

## 5️⃣ 20251019043404_SeedCountries

**Data:** 19/10/2025 04:34:04  
**Descrição:** Popula tabela Countries com 195 países

### Seed Data

```csharp
migrationBuilder.InsertData(
    table: "Countries",
    columns: new[] { "Name", "Code" },
    values: new object[,]
    {
        { "Brasil", "BRA" },
        { "Estados Unidos", "USA" },
        { "Reino Unido", "GBR" },
        { "França", "FRA" },
        { "Alemanha", "DEU" },
        // ... 190 países adicionais
    });
```

**Total:** 195 países com códigos ISO-3

### Estrutura

```sql
INSERT INTO Countries (Name, Code) VALUES 
    ('Afeganistão', 'AFG'),
    ('África do Sul', 'ZAF'),
    ('Albânia', 'ALB'),
    -- ... etc
```

---

## 6️⃣ 20251019045937_AddUniqueGenreNameIndex

**Data:** 19/10/2025 04:59:37  
**Descrição:** Adiciona índice único ao nome do gênero para evitar duplicatas

### Alteração

```sql
CREATE UNIQUE INDEX IX_Genres_Name ON Genres(Name);
```

**Efeito:**
- Impede criar gêneros com o mesmo nome
- Erro ao tentar inserir "Ficção" se já existe
- Melhora performance de queries por nome

**Implementação no Model:**
```csharp
[Index(nameof(Name), IsUnique = true)]
public class GenreModel
{
    [Required, MaxLength(50)]
    public string Name { get; set; }
}
```

---

## 7️⃣ 20251020030731_ChangeRatingToInteger

**Data:** 20/10/2025 03:07:31  
**Descrição:** Altera tipo de Rating de `decimal(2,1)` para `int`

### Alteração

```sql
-- Antes
ALTER TABLE Readings ALTER COLUMN Rating decimal(2,1) NULL;

-- Depois
ALTER TABLE Readings ALTER COLUMN Rating int NULL;
```

### Justificativa

**Antes:** Rating podia ser 0.0, 0.5, 1.0, 1.5, ... 5.0 (11 valores)  
**Depois:** Rating pode ser 0, 1, 2, 3, 4, 5 (6 valores)

**Motivo da mudança:**
- Sistema de estrelas inteiras é mais comum
- Simplifica UI (menos opções)
- Alinha com padrões de avaliação (0-5 estrelas)

### Validação

```csharp
[Range(0, 5, ErrorMessage = "A avaliação deve estar entre 0 e 5")]
public int? Rating { get; set; }
```

---

## 📋 Índices Criados

### Índices Únicos

| Tabela | Colunas | Nome | Propósito |
|--------|---------|------|-----------|
| Countries | Code | IX_Countries_Code | Código ISO único |
| Genres | Name | IX_Genres_Name | Nome do gênero único |
| BookGenres | BookId, GenreId | IX_BookGenres_BookId_GenreId | Evitar duplicatas N:N |
| Books | Title, AuthorId | IX_Books_Title_AuthorId | Livro único por autor |

### Índices de Performance

| Tabela | Colunas | Nome | Propósito |
|--------|---------|------|-----------|
| Authors | Name | IX_Authors_Name | Busca rápida por nome |
| Readings | UserId, BookId, Year, Month | IX_Readings_UserId_BookId_Year_Month | Queries do dashboard |

---

## 🔄 Comandos Úteis

### Criar Nova Migration

```bash
dotnet ef migrations add NomeDaMigration
```

### Aplicar Migrations

```bash
# Aplicar todas as migrations pendentes
dotnet ef database update

# Aplicar até uma migration específica
dotnet ef database update 20251019043404_SeedCountries

# Reverter todas (cuidado!)
dotnet ef database update 0
```

### Remover Última Migration

```bash
# Se ainda não aplicou ao banco
dotnet ef migrations remove

# Se já aplicou, reverter primeiro
dotnet ef database update PreviousMigration
dotnet ef migrations remove
```

### Gerar Script SQL

```bash
# Script de todas as migrations
dotnet ef migrations script

# Script de uma migration específica
dotnet ef migrations script 20251019043404_SeedCountries

# Script de migration range
dotnet ef migrations script FromMigration ToMigration
```

### Ver Migrations Aplicadas

```bash
dotnet ef migrations list
```

---

## ⚠️ Cuidados ao Modificar

### Antes de Criar Migration

1. ✅ Verifique se os Models estão corretos
2. ✅ Compile o projeto (`dotnet build`)
3. ✅ Teste localmente antes de aplicar em produção

### Ao Modificar Migration Existente

⚠️ **NUNCA modifique migrations já aplicadas em produção!**

Se precisar alterar:
1. Crie uma **nova migration** com as mudanças
2. Use `Up()` e `Down()` adequadamente
3. Teste em ambiente de desenvolvimento primeiro

### Ao Excluir Dados

```csharp
protected override void Down(MigrationBuilder migrationBuilder)
{
    // Sempre implemente o Down() para reverter
    migrationBuilder.DropTable("TableName");
    
    // Ou para dados
    migrationBuilder.DeleteData(
        table: "Countries",
        keyColumn: "Code",
        keyValue: "BRA"
    );
}
```

---

## 📚 Próximos Passos

- [Entenda os Models](../models.md)
- [Veja a estrutura do projeto](../architecture/project-structure.md)
- [AppDbContext Documentation](../architecture/patterns.md)

---

**Nota:** Migrations são versionadas no controle de código (Git). Sempre commite as migrations junto com as mudanças nos Models!

# 🗺️ Rotas do Sistema

Mapeamento completo de todas as rotas disponíveis no **gaby.io**.

## 🏠 Home / Dashboard

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/` | Home | Index | Pública | Dashboard com estatísticas |
| GET | `/Home` | Home | Index | Pública | Redireciona para `/` |
| GET | `/Home/Index` | Home | Index | Pública | Dashboard com gráficos |

---

## 🔐 Autenticação (Account)

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/Account/Register` | Account | Register | Pública | Formulário de registro |
| POST | `/Account/Register` | Account | Register | Pública | Criar nova conta |
| GET | `/Account/Login` | Account | Login | Pública | Formulário de login |
| POST | `/Account/Login` | Account | Login | Pública | Autenticar usuário |
| POST | `/Account/Logout` | Account | Logout | Autenticado | Deslogar usuário |
| GET | `/Account/Index` | Account | Index | Autenticado | Perfil do usuário |

---

## 📚 Livros (Books)

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/Books` | Book | Index | Autenticado | Listar todos os livros |
| GET | `/Books/Index` | Book | Index | Autenticado | Listar todos os livros |
| GET | `/Books/Details/{id}` | Book | Details | Autenticado | Detalhes do livro + avaliação média |
| GET | `/Books/Create` | Book | Create | Autenticado | Formulário criar livro |
| POST | `/Books/Create` | Book | Create | Autenticado | Salvar novo livro |
| GET | `/Books/Edit/{id}` | Book | Edit | Autenticado | Formulário editar livro |
| POST | `/Books/Edit/{id}` | Book | Edit | Autenticado | Atualizar livro |
| GET | `/Books/Delete/{id}` | Book | Delete | Autenticado | Confirmar exclusão |
| POST | `/Books/Delete/{id}` | Book | DeleteConfirmed | Autenticado | Excluir livro |

**Exemplos:**
```
GET  /Books                    → Lista todos os livros
GET  /Books/Details/5          → Detalhes do livro ID 5
POST /Books/Create             → Criar novo livro
GET  /Books/Edit/5             → Editar livro ID 5
POST /Books/Delete/5           → Excluir livro ID 5
```

---

## ✍️ Autores (Authors)

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/Authors` | Author | Index | Autenticado | Listar todos os autores |
| GET | `/Authors/Details/{id}` | Author | Details | Autenticado | Detalhes do autor + livros |
| GET | `/Authors/Create` | Author | Create | Autenticado | Formulário criar autor |
| POST | `/Authors/Create` | Author | Create | Autenticado | Salvar novo autor |
| GET | `/Authors/Edit/{id}` | Author | Edit | Autenticado | Formulário editar autor |
| POST | `/Authors/Edit/{id}` | Author | Edit | Autenticado | Atualizar autor |
| GET | `/Authors/Delete/{id}` | Author | Delete | Autenticado | Confirmar exclusão |
| POST | `/Authors/Delete/{id}` | Author | DeleteConfirmed | Autenticado | Excluir autor |

**Restrições:**
- Não pode excluir autor que possui livros cadastrados (OnDelete Restrict)

---

## 📖 Leituras (Readings)

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/Readings` | Reading | Index | Autenticado | Listar leituras do usuário |
| GET | `/Readings/Details/{id}` | Reading | Details | Próprio usuário | Detalhes da leitura |
| GET | `/Readings/Create` | Reading | Create | Autenticado | Formulário criar leitura |
| POST | `/Readings/Create` | Reading | Create | Autenticado | Salvar nova leitura |
| GET | `/Readings/Edit/{id}` | Reading | Edit | Próprio usuário | Formulário editar leitura |
| POST | `/Readings/Edit/{id}` | Reading | Edit | Próprio usuário | Atualizar leitura |
| GET | `/Readings/Delete/{id}` | Reading | Delete | Próprio usuário | Confirmar exclusão |
| POST | `/Readings/Delete/{id}` | Reading | DeleteConfirmed | Próprio usuário | Excluir leitura |

**Regras de Acesso:**
- Usuário só pode ver/editar/excluir suas próprias leituras
- Tentativa de acessar leitura de outro usuário retorna **403 Forbidden**

**Exemplo de verificação:**
```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
if (reading.UserId != userId)
{
    return Forbid(); // 403
}
```

---

## 🏢 Editoras (Publishers)

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/Publishers` | Publisher | Index | Autenticado | Listar todas as editoras |
| GET | `/Publishers/Details/{id}` | Publisher | Details | Autenticado | Detalhes da editora + livros |
| GET | `/Publishers/Create` | Publisher | Create | Autenticado | Formulário criar editora |
| POST | `/Publishers/Create` | Publisher | Create | Autenticado | Salvar nova editora |
| GET | `/Publishers/Edit/{id}` | Publisher | Edit | Autenticado | Formulário editar editora |
| POST | `/Publishers/Edit/{id}` | Publisher | Edit | Autenticado | Atualizar editora |
| GET | `/Publishers/Delete/{id}` | Publisher | Delete | Autenticado | Confirmar exclusão |
| POST | `/Publishers/Delete/{id}` | Publisher | DeleteConfirmed | Autenticado | Excluir editora |

**Restrições:**
- Ao excluir editora, livros associados ficam sem editora (OnDelete SetNull)

---

## 🎨 Gêneros (Genres)

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/Genres` | Genre | Index | Autenticado | Listar todos os gêneros |
| GET | `/Genres/Details/{id}` | Genre | Details | Autenticado | Detalhes do gênero + livros |
| GET | `/Genres/Create` | Genre | Create | Autenticado | Formulário criar gênero |
| POST | `/Genres/Create` | Genre | Create | Autenticado | Salvar novo gênero |
| GET | `/Genres/Edit/{id}` | Genre | Edit | Autenticado | Formulário editar gênero |
| POST | `/Genres/Edit/{id}` | Genre | Edit | Autenticado | Atualizar gênero |
| GET | `/Genres/Delete/{id}` | Genre | Delete | Autenticado | Confirmar exclusão |
| POST | `/Genres/Delete/{id}` | Genre | DeleteConfirmed | Autenticado | Excluir gênero |

**Restrições:**
- Nome do gênero deve ser único (índice único no banco)
- Ao excluir gênero, remove associações BookGenre (OnDelete Cascade)

---

## 🌍 Países (Countries)

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/Countries` | Country | Index | Autenticado | Listar todos os países |
| GET | `/Countries/Details/{id}` | Country | Details | Autenticado | Detalhes do país + autores |
| GET | `/Countries/Create` | Country | Create | Autenticado | Formulário criar país |
| POST | `/Countries/Create` | Country | Create | Autenticado | Salvar novo país |
| GET | `/Countries/Edit/{id}` | Country | Edit | Autenticado | Formulário editar país |
| POST | `/Countries/Edit/{id}` | Country | Edit | Autenticado | Atualizar país |
| GET | `/Countries/Delete/{id}` | Country | Delete | Autenticado | Confirmar exclusão |
| POST | `/Countries/Delete/{id}` | Country | DeleteConfirmed | Autenticado | Excluir país |

**Observações:**
- Seed automático de 195 países na migration
- Raramente precisa de CRUD manual
- Ao excluir país, autores ficam sem país (OnDelete SetNull)

---

## 🔐 Administração (Admin)

| Método | Rota | Controller | Action | Autorização | Descrição |
|--------|------|------------|--------|-------------|-----------|
| GET | `/Admin` | Admin | Index | **Apenas Admin** | Dashboard administrativo |
| GET | `/Admin/Index` | Admin | Index | **Apenas Admin** | Dashboard administrativo |
| GET | `/Admin/Users` | Admin | Users | **Apenas Admin** | Listar todos os usuários |
| POST | `/Admin/ToggleAdmin` | Admin | ToggleAdmin | **Apenas Admin** | Conceder/remover Admin |
| GET | `/Admin/AccessDenied` | Admin | AccessDenied | Pública | Página de acesso negado |

**Segurança:**
- Todas as rotas (exceto AccessDenied) requerem role **Admin**
- Admin não pode remover próprio papel de administrador
- Tentativa de acesso sem permissão redireciona para `/Admin/AccessDenied`

**Exemplo de uso:**
```
GET  /Admin/Users          → Lista usuários (apenas admin)
POST /Admin/ToggleAdmin    → Body: { userId: "abc123" }
```

---

## 🎯 Padrão de Rotas

O projeto usa o padrão MVC default:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### Exemplos:

| URL | Controller | Action | Parâmetro |
|-----|------------|--------|-----------|
| `/` | Home | Index | - |
| `/Books` | Book | Index | - |
| `/Books/Details/5` | Book | Details | id=5 |
| `/Authors/Edit/10` | Author | Edit | id=10 |
| `/Account/Login` | Account | Login | - |

### Rotas Personalizadas

Não há rotas personalizadas configuradas. Todas seguem o padrão default.

---

## 🔒 Níveis de Autorização

### Pública (AllowAnonymous)

```
GET  /
GET  /Home/Index
GET  /Account/Login
POST /Account/Login
GET  /Account/Register
POST /Account/Register
GET  /Admin/AccessDenied
```

### Autenticado (Authorize)

Todas as rotas de:
- Books
- Authors
- Publishers
- Genres
- Countries
- Readings (+ verificação de ownership)

### Admin Apenas (Authorize(Roles = "Admin"))

```
GET  /Admin
GET  /Admin/Index
GET  /Admin/Users
POST /Admin/ToggleAdmin
```

---

## 🚦 Códigos de Status HTTP

| Código | Situação | Exemplo |
|--------|----------|---------|
| **200 OK** | Sucesso | GET /Books |
| **302 Found** | Redirect | POST /Books/Create → Redirect /Books |
| **400 Bad Request** | ModelState inválido | POST com dados inválidos |
| **401 Unauthorized** | Não autenticado | GET /Books sem login |
| **403 Forbidden** | Sem permissão | GET /Admin/Users (sem ser admin) |
| **404 Not Found** | Recurso não existe | GET /Books/Details/999999 |
| **500 Internal Server Error** | Erro não tratado | Exception não capturada |

---

## 🔗 Geração de URLs

### Nas Views (Razor)

```html
<!-- Link para action -->
<a asp-controller="Book" asp-action="Details" asp-route-id="5">Ver Livro</a>

<!-- Link para Index (padrão) -->
<a asp-controller="Books" asp-action="Index">Todos os Livros</a>

<!-- Form POST -->
<form asp-controller="Book" asp-action="Create" method="post">
    @Html.AntiForgeryToken()
    <!-- campos -->
</form>
```

### Nos Controllers

```csharp
// RedirectToAction
return RedirectToAction(nameof(Index));
return RedirectToAction("Details", new { id = book.Id });
return RedirectToAction("Index", "Home");

// Url.Action
var url = Url.Action("Details", "Book", new { id = 5 });
// Retorna: "/Books/Details/5"
```

---

## 📚 Próximos Passos

- [Documentação dos Controllers](controllers.md)
- [ViewModels Reference](viewmodels.md)
- [Guia de Autenticação](../guides/authentication.md)

---

**Padrão de Rota:** `/{controller=Home}/{action=Index}/{id?}`

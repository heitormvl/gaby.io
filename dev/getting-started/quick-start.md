# 🚀 Guia de Início Rápido

Este guia vai te ajudar a colocar o **gaby.io** em execução rapidamente.

## ⚡ Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- ✅ [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- ✅ [Docker](https://www.docker.com/) (para rodar o PostgreSQL localmente) ou uma conta [Supabase](https://supabase.com/)
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- ✅ [Git](https://git-scm.com/)

## 📥 1. Clone o Repositório

```bash
git clone https://github.com/heitormvl/gaby.io.git
cd gaby.io
```

## 🔧 2. Configure a String de Conexão

Suba um PostgreSQL local via Docker:

```bash
docker run -e POSTGRES_PASSWORD=DevLocal!Passw0rd -e POSTGRES_DB=gabyio \
  -p 5432:5432 --name gabyio-postgres \
  -d postgres:16
```

Edite o arquivo `appsettings.Development.json` (ou use User Secrets) e ajuste a string de conexão:

```json
{
  "ConnectionStrings": {
    "Default": "Host=127.0.0.1;Port=5432;Database=gabyio;Username=postgres;Password=DevLocal!Passw0rd"
  }
}
```

### Exemplos de Strings de Conexão:

**PostgreSQL Local (Docker):**
```
Host=127.0.0.1;Port=5432;Database=gabyio;Username=postgres;Password=DevLocal!Passw0rd
```

**Supabase (produção/staging):**
```
Host=db.<project-ref>.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=SuaSenha
```

## 🗄️ 3. Execute as Migrations

Abra o terminal na pasta do projeto e execute:

```bash
dotnet ef database update
```

Isso irá:
- ✅ Criar o banco de dados `gabyio`
- ✅ Criar todas as tabelas necessárias
- ✅ Executar os seeds iniciais (países, roles)

## ▶️ 4. Execute o Projeto

```bash
dotnet run
```

Ou pressione `F5` no Visual Studio.

A aplicação estará disponível em:
- 🌐 HTTPS: `https://localhost:7001`
- 🌐 HTTP: `http://localhost:5000`

## 👤 5. Crie sua Conta

1. Acesse a aplicação no navegador
2. Clique em **"Registrar"**
3. Preencha seus dados:
   - Nome de exibição (ex: "Heitor")
   - Email
   - Senha (mínimo 4 caracteres)
4. Faça login

## 🔐 6. Torne-se Administrador

Para acessar o painel administrativo, você precisa criar o primeiro administrador.

Consulte o guia completo: [**Primeiro Acesso Admin**](../admin-quick-start.md)

**Método Rápido via SQL:**

Execute o script `dev/create-first-admin.sql` substituindo o email:

```sql
-- Substitua [SEU_EMAIL@example.com] pelo seu email
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id"
FROM "AspNetUsers" u, "AspNetRoles" r
WHERE u."Email" = 'SEU_EMAIL@example.com'
  AND r."Name" = 'Admin';
```

Faça logout e login novamente para atualizar suas permissões.

## 🎉 Pronto!

Agora você pode:

- 📚 Adicionar livros, autores e editoras
- 📖 Registrar suas leituras
- ⭐ Avaliar os livros que leu
- 📊 Ver suas estatísticas no dashboard
- 🔐 Gerenciar usuários (se for admin)

## 📚 Próximos Passos

- [Entenda a arquitetura do projeto](../architecture/project-structure.md)
- [Saiba mais sobre o sistema de avaliações](../rating-system.md)
- [Configure o painel administrativo](../admin-management.md)

## ❓ Problemas Comuns

### Erro: "Cannot connect to PostgreSQL" / "Connection refused"

- ✅ Verifique se o container Docker do PostgreSQL está em execução (`docker ps`)
- ✅ Confirme a string de conexão no `appsettings.Development.json`
- ✅ Teste a conexão com `psql` ou pgAdmin

### Erro: "No migrations found"

Execute:
```bash
dotnet restore
dotnet build
dotnet ef database update
```

### Erro: "Port already in use"

Edite `Properties/launchSettings.json` e altere as portas:
```json
"applicationUrl": "https://localhost:7002;http://localhost:5001"
```

---

**Precisa de ajuda?** Abra uma issue no GitHub!

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

Edite o arquivo `appsettings.json` e ajuste a string de conexão para seu SQL Server:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=GabyIO;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Exemplos de Strings de Conexão:

**SQL Server Local (Windows Authentication):**
```
Server=localhost;Database=GabyIO;Trusted_Connection=True;TrustServerCertificate=True
```

**SQL Server com Usuário/Senha:**
```
Server=localhost;Database=GabyIO;User Id=sa;Password=SuaSenha123;TrustServerCertificate=True
```

**SQL Server Express:**
```
Server=localhost\\SQLEXPRESS;Database=GabyIO;Trusted_Connection=True;TrustServerCertificate=True
```

## 🗄️ 3. Execute as Migrations

Abra o terminal na pasta do projeto e execute:

```bash
dotnet ef database update
```

Isso irá:
- ✅ Criar o banco de dados `GabyIO`
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
DECLARE @UserId NVARCHAR(450)
DECLARE @RoleId NVARCHAR(450)

SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'SEU_EMAIL@example.com'
SELECT @RoleId = Id FROM AspNetRoles WHERE Name = 'Admin'

IF @UserId IS NOT NULL AND @RoleId IS NOT NULL
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@UserId, @RoleId)
END
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

### Erro: "Cannot connect to SQL Server"

- ✅ Verifique se o SQL Server está em execução
- ✅ Confirme a string de conexão no `appsettings.json`
- ✅ Teste a conexão com SQL Server Management Studio

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

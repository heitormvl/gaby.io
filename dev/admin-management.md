# Gerenciamento de Administradores

## Como criar o primeiro administrador

Existem duas formas de criar o primeiro administrador do sistema:

### Opção 1: Via código (automático)

Edite o arquivo `Data/RoleSeed.cs` e descomente as linhas que criam o usuário admin padrão. Isso criará automaticamente um usuário com as seguintes credenciais ao iniciar a aplicação:

- **Email:** admin@gaby.io
- **Senha:** Admin123!

### Opção 2: Via banco de dados (manual)

1. Registre um usuário normalmente pela interface de registro
2. Conecte-se ao banco de dados PostgreSQL (via Supabase SQL Editor, `psql` ou pgAdmin)
3. Execute o seguinte script SQL, substituindo `[EMAIL_DO_USUARIO]` pelo email do usuário:

```sql
-- 1. Criar a role Admin (se não existir)
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
SELECT gen_random_uuid()::text, 'Admin', 'ADMIN', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'Admin');

-- 2. Adicionar o usuário à role Admin
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id"
FROM "AspNetUsers" u, "AspNetRoles" r
WHERE u."Email" = '[EMAIL_DO_USUARIO]'
  AND r."Name" = 'Admin'
  AND NOT EXISTS (
      SELECT 1 FROM "AspNetUserRoles" ur
      WHERE ur."UserId" = u."Id" AND ur."RoleId" = r."Id"
  );
```

## Acessando o painel administrativo

Após ter um usuário com o papel de Admin:

1. Faça login com as credenciais de administrador
2. Clique no link "Admin" que aparecerá na barra de navegação
3. Acesse "Gerenciar Usuários" para conceder ou remover papéis de administrador de outros usuários

## Recursos disponíveis para administradores

- **Gerenciamento de Usuários:** Listar todos os usuários e gerenciar seus papéis
- **Conceder papel de Admin:** Transformar usuários comuns em administradores
- **Remover papel de Admin:** Remover privilégios administrativos de usuários

## Segurança

- Apenas usuários com o papel "Admin" podem acessar o painel administrativo
- Administradores não podem remover o próprio papel de administrador
- Todas as ações são protegidas por tokens anti-forgery

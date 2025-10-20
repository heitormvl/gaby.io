# 🚀 Guia Rápido - Sistema de Administradores

## ⚡ Início Rápido

### 1️⃣ Criar o Primeiro Admin

**Opção mais rápida - Via SQL:**

1. Registre um usuário normalmente no site
2. Abra o SQL Server Management Studio (ou qualquer cliente SQL)
3. Abra o arquivo `dev/create-first-admin.sql`
4. Altere a linha `DECLARE @UserEmail NVARCHAR(256) = '[EMAIL_DO_USUARIO]'`
5. Execute o script
6. Pronto! O usuário agora é admin

**Exemplo:**
```sql
DECLARE @UserEmail NVARCHAR(256) = 'seu@email.com' -- ALTERE AQUI
```

### 2️⃣ Acessar o Painel Admin

1. Faça logout e login novamente com a conta que virou admin
2. Você verá um link "Admin" no menu superior
3. Clique em "Admin" → "Gerenciar Usuários"

### 3️⃣ Gerenciar Outros Usuários

Na tela de gerenciamento de usuários:
- **Tornar Admin**: Clique no botão verde "Tornar Admin"
- **Remover Admin**: Clique no botão vermelho "Remover Admin"

## 📋 URLs Importantes

- Painel Admin: `/Admin/Index`
- Gerenciar Usuários: `/Admin/Users`

## ⚠️ Importante

- Você **não pode** remover o próprio papel de admin (proteção)
- Apenas admins podem acessar essas páginas
- Todas as mudanças são confirmadas antes de serem aplicadas

## 🔍 Verificar Admins no Banco

Execute no SQL Server:

```sql
SELECT 
    u.Email,
    u.DisplayName,
    r.Name as Role
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Admin'
```

## 🆘 Problemas Comuns

**"Acesso Negado" ao tentar acessar /Admin**
- Faça logout e login novamente após virar admin
- Verifique se o usuário está na role Admin no banco de dados

**Link "Admin" não aparece no menu**
- Faça logout e login novamente
- Limpe o cache do navegador (Ctrl+Shift+Delete)

**Erro ao executar o script SQL**
- Verifique se o email está correto
- Verifique se o usuário existe no banco (tabela AspNetUsers)

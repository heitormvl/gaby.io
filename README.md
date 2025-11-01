# gaby.io

Sistema de gerenciamento de biblioteca pessoal desenvolvido em ASP.NET Core MVC.

## 🚀 Funcionalidades

- 📚 Gerenciamento de livros, autores e editoras
- 📖 Registro de leituras e lista de desejos
- 🎯 Sistema de avaliação de livros
- 👥 Sistema de autenticação e autorização
- 🔐 **Painel administrativo com gerenciamento de usuários**

## 🔐 Sistema de Administração

### Criar o Primeiro Administrador

1. Registre um usuário no sistema
2. Execute o script SQL em `dev/create-first-admin.sql` (alterando o email do usuário)
3. Faça logout e login novamente

Consulte a documentação completa em:
- [`dev/admin-quick-start.md`](dev/admin-quick-start.md) - Guia rápido
- [`dev/admin-management.md`](dev/admin-management.md) - Documentação completa
- [`dev/admin-system-summary.md`](dev/admin-system-summary.md) - Resumo técnico da implementação

### Recursos do Admin

- Listar todos os usuários do sistema
- Conceder papel de administrador
- Remover papel de administrador
- Interface amigável com confirmações

## 🛠️ Tecnologias

- ASP.NET Core 8.0 MVC
- Entity Framework Core
- SQL Server
- Identity (autenticação e autorização)
- Bootstrap 5
- Font Awesome

## 📦 Estrutura do Projeto

```
gaby.io/
├── Controllers/        # Controllers MVC
│   ├── AdminController.cs    # ⭐ Gerenciamento de usuários
│   ├── AccountController.cs
│   ├── BookController.cs
│   └── ...
├── Models/            # Modelos de dados
├── ViewModels/        # ViewModels
├── Views/             # Views Razor
│   ├── Admin/         # ⭐ Views administrativas
│   └── ...
├── Data/              # Contexto e seeds
│   ├── RoleSeed.cs    # ⭐ Seed de roles
│   └── ...
└── dev/               # 📚 Documentação completa
    ├── README.md                  # Índice da documentação
    ├── getting-started/           # Guias de início
    ├── architecture/              # Arquitetura do projeto
    ├── guides/                    # Guias de funcionalidades
    └── reference/                 # Referência técnica
```

## 🚀 Como Executar

### Início Rápido

1. Clone o repositório
2. Configure a string de conexão em `appsettings.json`
3. Execute as migrations: `dotnet ef database update`
4. Execute o projeto: `dotnet run`
5. Crie o primeiro admin seguindo `dev/admin-quick-start.md`

📖 **Guia Completo:** Consulte [`dev/getting-started/quick-start.md`](dev/getting-started/quick-start.md) para instruções detalhadas

## � Documentação

Documentação completa disponível em [`dev/`](dev/):

- **[Guia de Início Rápido](dev/getting-started/quick-start.md)** - Execute o projeto em 5 minutos
- **[Arquitetura](dev/architecture/project-structure.md)** - Estrutura e padrões do código
- **[Sistema de Autenticação](dev/guides/authentication.md)** - Identity e autorização
- **[Referência de Controllers](dev/reference/controllers.md)** - Documentação técnica completa
- **[Stack Tecnológica](dev/stack.md)** - Tecnologias utilizadas

📖 **Índice completo:** [`dev/README.md`](dev/README.md)

## �📝 Licença

Este projeto foi desenvolvido para fins educacionais.

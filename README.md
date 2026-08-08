# gaby.io

Sistema de gerenciamento de biblioteca pessoal desenvolvido com ASP.NET Core MVC, utilizando PostgreSQL como banco de dados e deployado na infraestrutura Fly.io.

## Funcionalidades

- Gerenciamento de livros, autores e editoras
- Registro de leituras e lista de desejos
- Sistema de avaliação de livros (0 a 5 estrelas)
- Autenticação e autorização via ASP.NET Core Identity
- Painel administrativo com gerenciamento de usuários
- Dashboard interativo com estatísticas de leitura

## Sistema de Administração

### Criar o Primeiro Administrador

1. Registre um usuário no sistema
2. Execute o script SQL em `dev/create-first-admin.sql` (alterando o email do usuário)
3. Faça logout e login novamente

### Recursos do Admin

- Listar todos os usuários do sistema
- Conceder e remover papel de administrador
- Interface com confirmações de ação

Consulte a documentação completa em:
- [`dev/admin-management.md`](dev/admin-management.md) - Documentação completa
- [`dev/admin-quick-start.md`](dev/admin-quick-start.md) - Guia rápido
- [`dev/README.md`](dev/README.md) - Índice da documentação

## Tecnologias

### Backend
- **ASP.NET Core:** 8.0 MVC
- **Entity Framework Core:** 8.0 (ORM)
- **C#:** 12.0
- **Npgsql.EntityFrameworkCore.PostgreSQL:** 8.0.11 (provider do PostgreSQL)

### Frontend
- **Bootstrap:** 5.3.2 (CSS Framework responsivo)
- **Chart.js:** 4.4.0 (visualizações estatísticas)
- **Font Awesome:** 6.5.1 (ícones vetoriais)

### Banco de Dados
- **PostgreSQL:** 15+ (via Supabase em produção, Docker local)

## Arquitetura de Infraestrutura

### Fly.io (Produção)

O projeto é configurado para ser executado na plataforma Fly.io, uma plataforma de infraestrutura global para aplicações cloud.

```
fly.toml Configuration:
- Região Principal: gru (São Paulo, Brasil)
- Tamanho da Máquina: shared-cpu-1x
- Memória: 512 MB
- HTTPS: Forçado (TLS automático)
- Auto-stop de máquinas: Ativado para economia de recursos
- Auto-start de máquinas: Ativado (inicia automaticamente ao receber requisições)
```

### Estrutura de Deploy

```
┌───────────────────────────────┐
│        Fly.io (GRU)           │
│  Região: São Paulo, Brasil    │
├───────────────────────────────┤
│  VM: shared-cpu-1x / 512 MB   │
│                              │
│  ┌───────────────────────┐    │
│  │ HTTP Service (8080)   │    │
│  │ Force HTTPS: true     │    │
│  └───────────────────────┘    │
│                              │
│  ┌───────────────────────┐    │
│  │ ASP.NET Core App      │    │
│  │ Port: 8080            │    │
│  └───────────┬───────────┘    │
│              │                │
│  ┌───────────▼───────────┐    │
│  │ PostgreSQL (Supabase) │    │
│  │ Connection via config │    │
│  └───────────────────────┘    │
└───────────────────────────────┘
```

### Ambientes Suportados

| Ambiente | Configuração |
|----------|-------------|
| Desenvolvimento Local | Windows 10/11, macOS, Linux (Ubuntu/Fedora) — PostgreSQL via Docker |
| Produção | Fly.io (GRU) + Supabase PostgreSQL |

### Configuração de Variáveis de Ambiente

```json
{
  "ConnectionStrings": {
    "Default": ""
  },
  "GoogleBooks": {
    "ApiKey": ""
  }
}
```

Em produção, configure as variáveis de ambiente no painel do Fly.io:
- `ConnectionStrings__Default` — String de conexão PostgreSQL (Supabase)
- `GoogleBooks__ApiKey` — API Key para busca de metadados de livros

## Estrutura do Projeto

```
gaby.io/
├── Controllers/          # Lógica de negócio MVC (9 controllers)
├── Data/                 # DbContext e seeds (AppDbContext, RoleSeed)
├── Factories/            # Factories customizadas (UserClaimsPrincipalFactory)
├── Models/               # Entidades do banco (User, Book, Author, etc.)
├── Migrations/           # Histórico de 7 migrações EF Core
├── Services/             # Servicos de negócio (GoogleBooks, Wikidata)
├── ViewComponents/       # Componentes reutilizáveis
├── Views/                # Templates Razor
│   └── Admin/            # Views administrativas
├── ViewModels/           # DTOs para Views (6 view models)
├── wwwroot/              # Assets estáticos
│   ├── css/
│   ├── js/
│   └── lib/              # Bootstrap, jQuery, Chart.js
├── Properties/           # launchSettings.json
├── dev/                  # Documentação completa (19 arquivos)
│   ├── README.md                      # Índice da documentação
│   ├── stack.md                       # Stack tecnológico
│   ├── getting-started/               # Guias de início (quick-start, environment-setup)
│   ├── architecture/                  # Arquitetura (project-structure, patterns)
│   ├── guides/                        # Guias (authentication, dashboard)
│   ├── reference/                     # Referência técnica (controllers, routes, migrations)
│   └── CHANGELOG.md                   # Histórico de mudanças
├── Program.cs            # Configuração da aplicação (PostgreSQL, Identity, DI)
├── appsettings.json     # Configurações gerais e strings de conexão
├── Dockerfile            # Container para deploy
└── fly.toml              # Configuração Fly.io (GRU, 512MB)
```

## Como Executar

### Início Rápido (Desenvolvimento Local)

1. Clone o repositório
2. Configure a string de conexão em `appsettings.json` para PostgreSQL local
3. Execute as migrações:

```bash
dotnet ef database update
```

4. Execute o projeto:

```bash
dotnet run
```

5. Acesse `http://localhost:8000` e crie o primeiro administrador seguindo `dev/admin-quick-start.md`

### Deploy na Fly.io

1. Instale o CLI do Fly.io: https://fly.io/docs/launch/install

2. Inicie um novo projeto ou use o existente:

```bash
fly launch --name gaby-io
# ou
fly deploy --app-name gaby-io
```

3. Configure as variáveis de ambiente:

```bash
fly secret set ConnectionStrings__Default="host=..."
fly secret set GoogleBooks__ApiKey="..."
```

4. Deploy:

```bash
fly deploy --app-name gaby-io
```

5. Acesse a aplicação: `https://gaby.io.fly.app`

## Documentação

Documentação completa disponível em [`dev/`](dev/):

- [Guia de Início Rápido](dev/getting-started/quick-start.md) — Execute o projeto em 5 minutos
- [Configuração do Ambiente](dev/getting-started/environment-setup.md) — Requisitos e configurações (Win/Linux/Mac)
- [Arquitetura](dev/architecture/project-structure.md) — Estrutura e padrões do código
- [Stack Tecnológica](dev/stack.md) — Tecnologias utilizadas
- [Sistema de Autenticação](dev/guides/authentication.md) — Identity e autorização
- [Sistema de Avaliações](dev/rating-system.md) — Como funciona o sistema de ratings
- [Gerenciamento de Usuários](dev/admin-management.md) — Painel administrativo
- [Referência de Controllers](dev/reference/controllers.md) — Documentação técnica completa
- [Referência de Rotas](dev/reference/routes.md) — Mapeamento de rotas
- [Referência de Migrações](dev/reference/migrations.md) — Histórico de 7 migrações do banco

Índice completo: [`dev/README.md`](dev/README.md)

## Licença

Este projeto foi desenvolvido para fins educacionais.

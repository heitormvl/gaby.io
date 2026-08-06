# 🗺️ Mapa Rápido da Documentação

Guia visual para encontrar rapidamente o que você precisa na documentação do **gaby.io**.

```
📁 dev/
│
├─── 📘 START HERE
│    └─── README.md ........................... Índice principal
│    └─── CHANGELOG.md ........................ Histórico de atualizações
│
├─── 🚀 GETTING STARTED ........................ Começar a usar
│    ├─── quick-start.md ...................... ⚡ Executar em 5 minutos
│    └─── environment-setup.md ................ 🔧 Configurar ambiente
│
├─── 🏗️ ARCHITECTURE ........................... Entender estrutura
│    ├─── project-structure.md ................ 📂 Organização de pastas
│    └─── patterns.md ......................... 🎨 Padrões e convenções
│
├─── 📚 GUIDES ................................. Funcionalidades
│    ├─── authentication.md ................... 🔐 Login e permissões
│    └─── dashboard.md ........................ 📊 Gráficos e estatísticas
│
├─── 📖 REFERENCE .............................. Consulta técnica
│    ├─── controllers.md ...................... 🎮 Todos os controllers
│    ├─── routes.md ........................... 🗺️ Todas as rotas
│    └─── migrations.md ....................... 📋 Histórico do banco
│
├─── 🔐 ADMIN .................................. Painel administrativo
│    ├─── admin-quick-start.md ................ ⚡ Criar primeiro admin
│    ├─── admin-management.md ................. 📖 Gerenciar usuários
│    └─── create-first-admin.sql .............. 💾 Script SQL
│
├─── 📊 TECHNICAL .............................. Detalhes técnicos
│    ├─── stack.md ............................ ⚙️ Tecnologias usadas
│    ├─── models.md ........................... 🗄️ Estrutura do banco
│    └─── rating-system.md .................... ⭐ Sistema de avaliações
│
```

---

## 🎯 Busca Rápida por Tarefa

### "Quero executar o projeto"
→ `getting-started/quick-start.md`

### "Preciso configurar meu ambiente"
→ `getting-started/environment-setup.md`

### "Como funciona a autenticação?"
→ `guides/authentication.md`

### "Onde fica o código do BookController?"
→ `reference/controllers.md` → Seção BookController

### "Qual a rota para criar um livro?"
→ `reference/routes.md` → Seção Books

### "Como criar um administrador?"
→ `admin-quick-start.md`

### "Quais tecnologias são usadas?"
→ `stack.md`

### "Como funciona o banco de dados?"
→ `models.md` + `reference/migrations.md`

### "Estrutura de pastas do projeto"
→ `architecture/project-structure.md`

### "Quais padrões arquiteturais são usados?"
→ `architecture/patterns.md`

### "Como funciona o dashboard?"
→ `guides/dashboard.md`

### "Sistema de avaliações (ratings)"
→ `rating-system.md`

---

## 📊 Documentação por Nível

### 🌱 Iniciante

Comece aqui se é sua primeira vez:

1. `README.md` - Visão geral
2. `getting-started/quick-start.md` - Execute o projeto
3. `architecture/project-structure.md` - Entenda a organização
4. `admin-quick-start.md` - Crie seu primeiro admin

### 🌿 Intermediário

Você já executou o projeto e quer entender melhor:

1. `guides/authentication.md` - Sistema de login
2. `guides/dashboard.md` - Como funcionam os gráficos
3. `stack.md` - Tecnologias em detalhes
4. `models.md` - Estrutura completa do banco

### 🌳 Avançado

Você quer contribuir ou modificar o código:

1. `architecture/patterns.md` - Padrões e boas práticas
2. `reference/controllers.md` - Documentação técnica completa
3. `reference/routes.md` - Mapeamento de todas as rotas
4. `reference/migrations.md` - Evolução do banco de dados

---

## 🔍 Busca por Conceito

| Conceito | Arquivo |
|----------|---------|
| **ASP.NET Core MVC** | `architecture/patterns.md` |
| **Authentication** | `guides/authentication.md` |
| **Authorization** | `guides/authentication.md` |
| **Bootstrap** | `stack.md` |
| **Chart.js** | `guides/dashboard.md` |
| **Claims** | `guides/authentication.md` |
| **Controllers** | `reference/controllers.md` |
| **CRUD Operations** | `reference/controllers.md` |
| **Database** | `models.md` |
| **Entity Framework** | `stack.md`, `models.md` |
| **Identity** | `guides/authentication.md` |
| **Migrations** | `reference/migrations.md` |
| **Models** | `models.md` |
| **Patterns** | `architecture/patterns.md` |
| **Ratings** | `rating-system.md` |
| **Razor Views** | `architecture/project-structure.md` |
| **Roles** | `guides/authentication.md` |
| **Routes** | `reference/routes.md` |
| **Security** | `guides/authentication.md`, `stack.md` |
| **PostgreSQL** | `stack.md`, `getting-started/environment-setup.md` |
| **ViewModels** | `architecture/patterns.md` |

---

## 🎨 Legenda de Ícones

| Ícone | Significado |
|-------|-------------|
| 🚀 | Início rápido / Quick start |
| 🔧 | Configuração / Setup |
| 🏗️ | Arquitetura / Structure |
| 📚 | Guias / Guides |
| 📖 | Referência técnica |
| 🔐 | Segurança / Admin |
| 📊 | Dados / Statistics |
| ⚙️ | Tecnologias / Tech stack |
| 🗄️ | Banco de dados |
| ⭐ | Funcionalidades especiais |
| 📝 | Documentação geral |
| 🎯 | Objetivo / Target |
| ⚡ | Rápido / Fast |
| 🎮 | Controllers |
| 🗺️ | Rotas / Routing |
| 📋 | Listas / Histórico |
| 💾 | Scripts / SQL |

---

## 📞 Suporte

**Precisa de ajuda?**

1. Procure no mapa acima
2. Use Ctrl+F para buscar palavras-chave
3. Abra uma issue no GitHub
4. Entre em contato com a equipe

---

**Dica:** Mantenha este arquivo aberto em uma aba para referência rápida! 📌

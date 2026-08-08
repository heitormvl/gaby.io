# 🔧 Configuração do Ambiente

Guia detalhado para configurar seu ambiente de desenvolvimento para o **gaby.io**.

## 📋 Requisitos do Sistema

### Software Obrigatório

| Componente | Versão Mínima | Recomendado | Link |
|------------|---------------|-------------|------|
| .NET SDK | 8.0 | 8.0.x | [Download](https://dotnet.microsoft.com/download) |
| PostgreSQL | 15 | 16 | [Download](https://www.postgresql.org/download/) |
| C# | 12.0 | 12.0 | (Incluído no .NET SDK) |

### Ferramentas Recomendadas

- **IDE:**
  - Visual Studio 2022 (Community ou superior)
  - Visual Studio Code + C# Extension
  
- **Banco de Dados:**
  - Docker (para rodar o PostgreSQL localmente)
  - pgAdmin ou `psql`
  - Conta [Supabase](https://supabase.com/) (para o banco hospedado)
  
- **Controle de Versão:**
  - Git 2.0+

## 🪟 Windows

### 1. Instalar .NET SDK

```powershell
# Via winget (Windows 11)
winget install Microsoft.DotNet.SDK.8

# Ou baixe o instalador em:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

Verifique a instalação:
```powershell
dotnet --version
# Deve exibir: 8.0.x
```

### 2. Instalar PostgreSQL

**Opção A: Docker (Recomendado)**
```powershell
docker run -e POSTGRES_PASSWORD=DevLocal!Passw0rd -e POSTGRES_DB=gabyio `
  -p 5432:5432 --name gabyio-postgres `
  -d postgres:16
```

**Opção B: Instalador nativo**
- Baixe em: https://www.postgresql.org/download/windows/
- Durante a instalação, defina uma senha para o usuário `postgres`

### 3. Instalar Entity Framework Tools

```powershell
dotnet tool install --global dotnet-ef
```

Verifique:
```powershell
dotnet ef --version
# Deve exibir: 8.0.x
```

### 4. Configurar PostgreSQL

**String de conexão típica:**
```
Host=127.0.0.1;Port=5432;Database=gabyio;Username=postgres;Password=DevLocal!Passw0rd
```

## 🐧 Linux

### 1. Instalar .NET SDK

**Ubuntu/Debian:**
```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install dotnet-sdk-8.0
```

**Fedora:**
```bash
sudo dnf install dotnet-sdk-8.0
```

### 2. Instalar PostgreSQL no Linux

**Opção A: Docker (Recomendado)**
```bash
docker run -e POSTGRES_PASSWORD=DevLocal!Passw0rd -e POSTGRES_DB=gabyio \
  -p 5432:5432 --name gabyio-postgres \
  -d postgres:16
```

**Opção B: Instalação nativa**
```bash
# Ubuntu
sudo apt update
sudo apt install -y postgresql postgresql-contrib

# Fedora
sudo dnf install -y postgresql-server postgresql-contrib
sudo postgresql-setup --initdb
sudo systemctl enable --now postgresql
```

### 3. Instalar EF Tools

```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
```

## 🍎 macOS

### 1. Instalar .NET SDK

```bash
# Via Homebrew
brew install dotnet-sdk

# Ou baixe em:
# https://dotnet.microsoft.com/download
```

### 2. PostgreSQL no macOS

**Opção A: Docker (Recomendado)**
```bash
docker run -e POSTGRES_PASSWORD=DevLocal!Passw0rd -e POSTGRES_DB=gabyio \
  -p 5432:5432 --name gabyio-postgres \
  -d postgres:16
```

**Opção B: Homebrew**
```bash
brew install postgresql@16
brew services start postgresql@16
```

**String de conexão:**
```
Host=127.0.0.1;Port=5432;Database=gabyio;Username=postgres;Password=DevLocal!Passw0rd
```

### 3. Instalar EF Tools

```bash
dotnet tool install --global dotnet-ef
```

## 🔐 Variáveis de Ambiente (Opcional)

Para não expor credenciais no código, use User Secrets:

```bash
# Inicializar secrets
dotnet user-secrets init

# Adicionar connection string
dotnet user-secrets set "ConnectionStrings:Default" "Host=127.0.0.1;Port=5432;Database=gabyio;Username=postgres;Password=DevLocal!Passw0rd"
```

## 🧪 Verificar Instalação

Execute os comandos abaixo para validar o ambiente:

```bash
# Versão do .NET
dotnet --version

# Entity Framework Tools
dotnet ef --version

# Restaurar pacotes do projeto
dotnet restore

# Compilar o projeto
dotnet build

# Executar testes (se houver)
dotnet test
```

## 📦 Pacotes NuGet Utilizados

O projeto utiliza os seguintes pacotes principais:

```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.0" />
```

Eles são instalados automaticamente ao executar `dotnet restore`.

## 🐳 Configuração com Docker (Alternativa)

### docker-compose.yml

```yaml
version: '3.8'
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: "DevLocal!Passw0rd"
      POSTGRES_DB: "gabyio"
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

Execute:
```bash
docker-compose up -d
```

## ⚠️ Problemas Comuns

### "dotnet: command not found"

Adicione o .NET ao PATH:
```bash
export PATH="$PATH:/usr/local/share/dotnet"
```

### "Cannot connect to PostgreSQL"

- ✅ Verifique se o serviço (ou container Docker) está rodando
- ✅ Teste com `psql` ou pgAdmin
- ✅ Verifique o firewall
- ✅ Confirme a porta 5432

### "Migration failed"

```bash
# Limpar migrations antigas
dotnet ef database drop
dotnet ef database update
```

## 🔄 Próximos Passos

Agora que seu ambiente está configurado:

1. [Execute o projeto pela primeira vez](quick-start.md)
2. [Entenda a estrutura do código](../architecture/project-structure.md)
3. [Configure o primeiro administrador](../admin-quick-start.md)

---

**Dúvidas?** Consulte a [documentação oficial do .NET](https://learn.microsoft.com/dotnet/)

# 🔧 Configuração do Ambiente

Guia detalhado para configurar seu ambiente de desenvolvimento para o **gaby.io**.

## 📋 Requisitos do Sistema

### Software Obrigatório

| Componente | Versão Mínima | Recomendado | Link |
|------------|---------------|-------------|------|
| .NET SDK | 8.0 | 8.0.x | [Download](https://dotnet.microsoft.com/download) |
| SQL Server | 2019 | 2022 | [Download](https://www.microsoft.com/sql-server) |
| C# | 12.0 | 12.0 | (Incluído no .NET SDK) |

### Ferramentas Recomendadas

- **IDE:**
  - Visual Studio 2022 (Community ou superior)
  - Visual Studio Code + C# Extension
  
- **Banco de Dados:**
  - SQL Server Management Studio (SSMS)
  - Azure Data Studio
  
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

### 2. Instalar SQL Server

**Opção A: SQL Server Express (Gratuito)**
```powershell
# Baixe e instale:
# https://www.microsoft.com/sql-server/sql-server-downloads

# Escolha "Express" durante a instalação
```

**Opção B: SQL Server Developer (Gratuito para desenvolvimento)**
- Mais recursos que o Express
- Ideal para desenvolvimento local

### 3. Instalar Entity Framework Tools

```powershell
dotnet tool install --global dotnet-ef
```

Verifique:
```powershell
dotnet ef --version
# Deve exibir: 8.0.x
```

### 4. Configurar SQL Server

**Habilitar autenticação do Windows (recomendado):**

1. Abra o SQL Server Configuration Manager
2. Vá em "SQL Server Network Configuration" > "Protocols"
3. Habilite "TCP/IP"
4. Reinicie o serviço SQL Server

**String de conexão típica:**
```
Server=localhost;Database=GabyIO;Trusted_Connection=True;TrustServerCertificate=True
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

### 2. Instalar SQL Server no Linux

```bash
# Ubuntu
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/20.04/mssql-server-2022.list)"
sudo apt update
sudo apt install -y mssql-server
sudo /opt/mssql/bin/mssql-conf setup
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

### 2. SQL Server no macOS

**Opção A: Docker (Recomendado)**
```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=SuaSenha123!" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

**String de conexão:**
```
Server=localhost,1433;Database=GabyIO;User Id=sa;Password=SuaSenha123!;TrustServerCertificate=True
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
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Database=GabyIO;User Id=sa;Password=SuaSenha123!;TrustServerCertificate=True"
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
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
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
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "SuaSenha123!"
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
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

### "Cannot connect to SQL Server"

- ✅ Verifique se o serviço está rodando
- ✅ Teste com SSMS ou Azure Data Studio
- ✅ Verifique o firewall
- ✅ Confirme a porta 1433

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

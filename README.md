# FIAP Cloud Games

Plataforma de distribuição digital de jogos desenvolvida em .NET 9 com ASP.NET Core Web API.

---

## Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Docker](https://www.docker.com/get-started)

---

## 1. Banco de Dados (Docker)

Escolha o script de acordo com seu sistema operacional:

**Windows:**
```bat
cd DockerSqlServer
iniciar-servidor.bat
```

**Linux/macOS:**
```bash
cd DockerSqlServer
chmod +x iniciar-servidor.sh
./iniciar-servidor.sh
```

**SQL Server Local:**
Caso não seja possível rodar o Docker, basta configurar um SQL Server e ajustar a variável "ConexaoPadrao" do arquivo "appsettings.json" para uma string de conexão válida.

O banco `FCG` será criado automaticamente com todos os dados de exemplo na primeira execução.

Caso deseje, basta rodar o arquivo "Docs\FCG.sql" para criar um ambiente completo com dados para teste pré-cadastrados.

---

## 2. Rodar a API

```bash
cd src/FiapCloudGames/FIAP.FCG.Web.API
dotnet run
```

A API estará disponível em: `http://localhost:5295`

---

## Migrations

Foi utilizado engenharia reversa (via Scaffold-DbContext) para gerar os modelos com base no banco de dados já existente.

# Cadastro API — Dapper + SQL Server + Docker

API REST de cadastro de clientes construída com **ASP.NET Core 8**, **Dapper**, **SQL Server**, versionamento de API e Result Pattern.

---

## Estrutura do Projeto

```
Cadastro.API/
├── Controllers/          # Endpoints HTTP
├── Services/             # Regras de negócio
├── Repositories/         # Acesso a dados via Dapper
├── Models/               # Entidades do banco
├── DTOs/                 # Objetos de entrada e saída
└── Infra/
    ├── Common/           # Result Pattern (Result.cs, ResultExtensions.cs)
    └── Database/
        ├── ConnectionFactory.cs
        ├── IConnectionFactory.cs
        ├── Procedures/   # Constantes das Stored Procedures
        └── Scripts/      # Scripts SQL e entrypoint Docker
```

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup) *(opcional)*

---

## Pacotes utilizados

| Pacote                           | Finalidade                              |
|----------------------------------|-----------------------------------------|
| `Dapper`                         | Micro ORM para acesso a dados           |
| `Microsoft.Data.SqlClient`       | Driver SQL Server                       |
| `Asp.Versioning.Mvc`             | Versionamento de API                    |
| `Asp.Versioning.Mvc.ApiExplorer` | Integração do versionamento com Swagger |
| `Swashbuckle.AspNetCore`         | Documentação Swagger/OpenAPI            |

---

## Como subir com Docker Compose *(recomendado)*

O Docker Compose sobe **3 serviços automaticamente**:

| Serviço          | Descrição                             |
|------------------|---------------------------------------|
| `sqlserver`      | SQL Server 2022                       |
| `sqlserver-init` | Executa o script SQL de inicialização |
| `cadastro-api`   | A API .NET 8                          |

### Passo a passo

**1. Clone o repositório e acesse a pasta raiz:**
```bash
cd C:\Projetos\CadastroDapper
```

**2. Suba os containers:**
```bash
docker-compose up --build
```

**3. Aguarde** — o `sqlserver-init` tentará conectar no SQL Server a cada 5 segundos (até 30 tentativas) e executará o script automaticamente ao detectar que o banco está pronto.

**4. Acesse o Swagger:**
```
http://localhost:8080/swagger
```

### Derrubar os containers
```bash
docker-compose down
```

### Derrubar e remover o volume do banco (reset completo)
```bash
docker-compose down -v
```

---

## Como subir apenas o Docker (sem Compose)

**1. Build da imagem:**
```bash
docker build -f Cadastro.API/Dockerfile -t cadastro-api .
```

**2. Suba um SQL Server separado:**
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Senha@1234" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

**3. Suba a API apontando para o SQL Server:**
```bash
docker run -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=DapperCliente;User Id=sa;Password=Senha@1234;TrustServerCertificate=True;" \
  cadastro-api
```

**4. Execute o script SQL manualmente** no SSMS ou via sqlcmd:
```bash
docker exec -it sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Senha@1234" -C -i /scripts/Cliente.sql
```

---

## Scripts SQL

Localização: `Cadastro.API/Infra/Database/Scripts/`

| Arquivo         | Descrição                                                  |
|-----------------|------------------------------------------------------------|
| `Cliente.sql`   | Cria o banco, a tabela e todas as stored procedures        |
| `entrypoint.sh` | Script bash executado pelo Docker para inicializar o banco |

### O que o `Cliente.sql` faz

- Cria o banco `DapperCliente` se não existir
- Cria a tabela `Cliente` se não existir
- Cria as stored procedures com verificação de existência (`IF NOT EXISTS`)

### Stored Procedures

| Procedure              | Operação                                     |
|------------------------|----------------------------------------------|
| `sp_Cliente_Listar`    | Lista todos os clientes ativos               |
| `sp_Cliente_Obter`     | Busca um cliente por ID                      |
| `sp_Cliente_Inserir`   | Insere um novo cliente e retorna o ID gerado |
| `sp_Cliente_Atualizar` | Atualiza os dados de um cliente              |
| `sp_Cliente_Deletar`   | Soft delete — seta `Ativo = 0`               |

### Como o `entrypoint.sh` funciona

```bash
# Tenta conectar no SQL Server a cada 5 segundos
for i in {1..30}; do
    sqlcmd -S sqlserver -U sa -P "Senha@1234" -Q "SELECT 1"
    # Se conectou, executa o script e sai do loop
done

sqlcmd -S sqlserver -U sa -P "Senha@1234" -i /scripts/Cliente.sql
```

---

## Dapper com Stored Procedures

O Dapper é um micro ORM que executa SQL diretamente, sem abstração de queries. Neste projeto, **todas as operações passam por stored procedures**.

### ConnectionFactory

A `ConnectionFactory` centraliza a criação de conexões com o banco:

```csharp
public IDbConnection CreateConnection()
{
    return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
}
```

Registrada no DI como `Scoped`:
```csharp
builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();
```

### Constantes das Procedures

As procedures são referenciadas por constantes em `ClienteProcedures`, evitando strings espalhadas pelo código:

```csharp
public static class ClienteProcedures
{
    public const string ObterTodos = "sp_Cliente_Listar";
    public const string ObterPorId = "sp_Cliente_Obter";
    public const string Inserir    = "sp_Cliente_Inserir";
    public const string Atualizar  = "sp_Cliente_Atualizar";
    public const string Deletar    = "sp_Cliente_Deletar";
}
```

### Exemplos de uso no Repository

**Listar todos:**
```csharp
using var connection = _factory.CreateConnection();
return await connection.QueryAsync<Cliente>(
    ClienteProcedures.ObterTodos,
    commandType: CommandType.StoredProcedure);
```

**Buscar por ID:**
```csharp
using var connection = _factory.CreateConnection();
return await connection.QueryFirstOrDefaultAsync<Cliente>(
    ClienteProcedures.ObterPorId,
    new { Id = id },
    commandType: CommandType.StoredProcedure);
```

**Inserir e retornar ID:**
```csharp
using var connection = _factory.CreateConnection();
return await connection.ExecuteScalarAsync<int>(
    ClienteProcedures.Inserir,
    new { cliente.Nome, cliente.Email, cliente.Telefone, cliente.CPF, cliente.DataNascimento },
    commandType: CommandType.StoredProcedure);
```

---

## Endpoints da API

Base URL: `http://localhost:8080/api/v1`

| Método   | Rota            | Descrição                      |
|----------|-----------------|--------------------------------|
| `GET`    | `/cliente`      | Lista todos os clientes ativos |
| `GET`    | `/cliente/{id}` | Busca cliente por ID           |
| `POST`   | `/cliente`      | Cadastra novo cliente          |
| `PUT`    | `/cliente/{id}` | Atualiza cliente               |
| `DELETE` | `/cliente/{id}` | Remove cliente (soft delete)   |

### Exemplo de Request (POST)

```json
{
  "nome": "João Silva",
  "email": "joao@email.com",
  "telefone": "11999999999",
  "cpf": "12345678901",
  "dataNascimento": "1990-05-10"
}
```

### Exemplo de Response

```json
{
  "value": {
    "id": 1,
    "nome": "João Silva",
    "email": "joao@email.com",
    "telefone": "11999999999",
    "cpf": "12345678901",
    "dataNascimento": "1990-05-10T00:00:00",
    "dataCadastro": "2026-05-13T12:00:00",
    "ativo": true
  },
  "isSuccess": true,
  "error": null
}
```

### Response de erro

```json
{
    "value": null,
  "isSuccess": false,
  "error": "Cliente 99 não encontrado."
}
```

---

## Result Pattern

Todas as operações do `Service` retornam `Result<T>` ou `Result`, garantindo que erros sejam tratados de forma explícita sem uso de exceções para fluxo de negócio.

```csharp
// Sucesso
Result<ClienteResponse>.Success(response);

// Falha
Result<ClienteResponse>.Failure("Cliente não encontrado.");
```

No controller, o resultado é verificado via `IsSuccess`:
```csharp
return result.IsSuccess
    ? Ok(new { result.IsSuccess, result.Value, result.Error })
    : NotFound(new { result.IsSuccess, result.Value, result.Error });
```

---

## Versionamento de API

A API utiliza versionamento via URL. A versão atual é `v1`.

```
GET /api/v1/cliente
```

Para adicionar uma nova versão futuramente, basta criar um novo controller com:
```csharp
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
```

O Swagger exibirá automaticamente os dois grupos separados.

---

## Conexão com o banco (SSMS)

Para conectar no SQL Server do Docker via SSMS:

| Campo | Valor |
|----------------|-----------------------------|
| Server         | `localhost,1433`            |
| Authentication | `SQL Server Authentication` |
| Login          | `sa`                        |
| Password       | `Senha@1234`                |

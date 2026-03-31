# LeoMadeiras API

API REST em .NET 8 para gerenciamento de produtos e registro de vendas,
desenvolvida com Clean Architecture, Use Cases, SQL Server e Docker.

---

## Tecnologias

- .NET 8 — Web API com Controllers
- Entity Framework Core 8 — SQL Server
- JWT Bearer — autenticação
- Serilog — logs estruturados
- Swagger / Scalar — documentação interativa
- Docker + Docker Compose
- xUnit + Moq + FluentAssertions — testes

---

## Arquitetura
```
src/
├── LeoMadeiras.API            → Controllers, Middlewares, Program.cs
├── LeoMadeiras.Application    → Use Cases, DTOs, Interfaces
├── LeoMadeiras.Domain         → Entidades, Regras de negócio, Exceptions
└── LeoMadeiras.Infrastructure → EF Core, Repositories, Mappings, UnitOfWork

tests/
└── LeoMadeiras.Tests          → Testes unitários e de integração
```

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## Como executar

### Com Docker (recomendado)
```bash
# Na raiz do projeto
docker-compose up --build
```

A API sobe em `http://localhost:8080` e o SQL Server em `localhost,1433`.
As migrations são aplicadas automaticamente na inicialização da API.

### Sem Docker (local)

1. Suba um SQL Server local ou via Docker apenas o banco:
```bash
docker-compose up sqlserver
```

2. Configure a connection string no `appsettings.Development.json`

3. Aplique as migrations:
```bash
dotnet ef database update \
  --project src/LeoMadeiras.Infrastructure \
  --startup-project src/LeoMadeiras.API
```

4. Execute a API:
```bash
dotnet run --project src/LeoMadeiras.API
```

A API sobe em `http://localhost:5000`.

---

## Swagger

Acesse a documentação interativa em:
```
http://localhost:8080/swagger
```

---

## Autenticação

A API usa JWT Bearer. Para obter um token:
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "senha": "admin123"
}
```

Use o token retornado no header de todas as requisições protegidas:
```
Authorization: Bearer {token}
```

No Swagger, clique em **Authorize** e insira `Bearer {token}`.

---

## Endpoints

### Produtos

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | `/api/produtos` | Lista com filtros e paginação | Não |
| GET | `/api/produtos/{id}` | Busca por ID | Não |
| POST | `/api/produtos` | Cria produto | Sim |
| PUT | `/api/produtos/{id}` | Atualiza produto | Sim |
| DELETE | `/api/produtos/{id}` | Remove produto | Sim |
| GET | `/api/produtos/mais-vendidos` | Produtos mais vendidos | Não |

### Vendas

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/vendas` | Registra venda | Sim |

### Auth

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/login` | Gera token JWT |

---

## Filtros disponíveis — GET /api/produtos

| Parâmetro | Tipo | Descrição |
|-----------|------|-----------|
| `nome` | string | Filtra por nome (contains) |
| `precoMin` | decimal | Preço mínimo |
| `precoMax` | decimal | Preço máximo |
| `orderBy` | string | `nome`, `preco`, `preco_desc` |
| `page` | int | Página (default: 1) |
| `pageSize` | int | Itens por página (default: 10) |

---

## Regras de negócio

- Venda não permitida sem estoque disponível
- Quantidade deve ser maior que zero
- Estoque é debitado automaticamente após a venda
- O valor total é calculado no backend
- Operação de venda é transacional
- Vendas duplicadas são bloqueadas pelo campo `Order` (idempotência)

---

## Testes
```bash
dotnet test
```

### O que é testado

- Venda com sucesso — fluxo completo
- Venda sem estoque — deve retornar erro
- Quantidade zero ou negativa — deve retornar erro
- Venda duplicada — deve retornar erro
- Cálculo correto do total
- Atualização correta do estoque com múltiplos itens

---

## Migrations
```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration \
  --project src/LeoMadeiras.Infrastructure \
  --startup-project src/LeoMadeiras.API

# Aplicar migrations
dotnet ef database update \
  --project src/LeoMadeiras.Infrastructure \
  --startup-project src/LeoMadeiras.API

# Reverter última migration
dotnet ef migrations remove \
  --project src/LeoMadeiras.Infrastructure \
  --startup-project src/LeoMadeiras.API
```
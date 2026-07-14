# Product API

RESTful ASP.NET Core Web API for managing Products and Items with full CRUD operations, pagination, JWT authentication, and Swagger documentation.

## Tech Stack

- **.NET 8** — ASP.NET Core Web API
- **Entity Framework Core 8** — Code-First with SQL Server or SQLite fallback
- **Clean Architecture** — Domain → Application → Infrastructure → API
- **JWT Bearer** — Access + Refresh token authentication
- **FluentValidation** — Request validation
- **AutoMapper** — Object mapping
- **Serilog** — Structured logging (Console + File)
- **Swagger / Swashbuckle** — OpenAPI documentation
- **xUnit + Moq** — Unit and integration testing
- **Docker** — Containerized deployment with SQL Server

## Architecture

```
ProductApi.sln
├── src/
│   ├── Domain/              # Entities, Enums, Exceptions
│   ├── Application/         # DTOs, Interfaces, Services, Validators, Mapping
│   ├── Infrastructure/      # EF Core, DbContext, Repositories, UnitOfWork, Identity
│   └── API/                 # Controllers, Middleware, Filters, Extensions
└── tests/
    ├── Application.Tests/   # Unit tests (Moq)
    ├── Infrastructure.Tests/ # Repository tests (EF Core InMemory)
    └── API.Tests/           # Integration tests (WebApplicationFactory)
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional — for containerized SQL Server)
- SQL Server (local instance or Docker container)

## Running Locally

### Option 0: Quick Start (SQLite — no dependencies)

```bash
dotnet run --project src/API/ProductApi.API
```

The app will auto-create a SQLite database file (`ProductApi.db`) and start on `http://localhost:5237`. Open [http://localhost:5237/swagger](http://localhost:5237/swagger) to explore the API.

To switch to SQL Server, set `"DatabaseProvider": "SqlServer"` in `appsettings.json`.

### Option 1: Local SQL Server

1. **Update the connection string** in `src/API/ProductApi.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=ProductApiDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

2. **Apply migrations**:
   ```bash
   dotnet ef database update --project src/Infrastructure/ProductApi.Infrastructure --startup-project src/API/ProductApi.API
   ```

3. **Run the API**:
   ```bash
   dotnet run --project src/API/ProductApi.API
   ```

4. Open Swagger UI: [http://localhost:5237/swagger](http://localhost:5237/swagger)

### Option 2: Docker Compose (recommended)

1. **Start all services**:
   ```bash
   docker-compose up -d
   ```

2. The API is available at [http://localhost:5000/swagger](http://localhost:5000/swagger)

3. Migrations are applied automatically in development mode.

### Option 3: SQL Server via Docker + API locally

1. **Start SQL Server**:
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
   ```

2. **Update connection string** in `appsettings.Development.json`:
   ```json
   "DefaultConnection": "Server=localhost;Database=ProductApiDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
   ```

3. **Apply migrations and run** as in Option 1.

## Running Tests

```bash
dotnet test
```

This runs all 40+ tests across three test projects:
- **Application.Tests** — Service layer unit tests with mocked dependencies
- **Infrastructure.Tests** — Repository tests with EF Core InMemory provider
- **API.Tests** — Integration tests with `WebApplicationFactory<Program>`

## API Endpoints

### Products

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/products?pageNumber=1&pageSize=10` | No | Get paginated products |
| GET | `/api/v1/products/{id}` | No | Get product by ID (includes items) |
| POST | `/api/v1/products` | Yes | Create a new product |
| PUT | `/api/v1/products/{id}` | Yes | Update an existing product |
| DELETE | `/api/v1/products/{id}` | Yes | Delete a product |
| GET | `/api/v1/products/{id}/items?pageNumber=1&pageSize=10` | No | Get paginated items for a product |

### Items

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/items/{id}` | No | Get item by ID |
| POST | `/api/v1/items` | Yes | Create a new item |
| PUT | `/api/v1/items/{id}` | Yes | Update an item |
| DELETE | `/api/v1/items/{id}` | Yes | Delete an item |

### Pagination

All collection endpoints accept `pageNumber` and `pageSize` query parameters:
- `pageNumber` — defaults to 1, minimum 1
- `pageSize` — defaults to 10, minimum 1, maximum 100

### Error Responses

The API returns consistent JSON error responses:

```json
{
  "error": "Resource not found.",
  "detail": "Entity \"Product\" (00000000-0000-0000-0000-000000000000) was not found."
}
```

HTTP status codes:
- **400** — Validation failure
- **401** — Unauthorized (missing/invalid JWT)
- **404** — Resource not found
- **409** — Conflict
- **500** — Internal server error

## Authentication

The API uses JWT Bearer token authentication for mutating endpoints (POST, PUT, DELETE).

### Auth Flow

1. **Login** — Send credentials to `/api/auth/login` (not yet implemented — use your own identity provider or generate tokens via the JWT token service)
2. **Receive tokens** — Response includes an access token (15 min expiry) and refresh token
3. **Access resources** — Include the access token in the `Authorization: Bearer <token>` header
4. **Refresh** — When the access token expires, use the refresh token to get a new pair

### Swagger Auth

1. Open Swagger UI at `/swagger`
2. Click the **Authorize** button
3. Enter your JWT token: `Bearer <your-token>`
4. Mutating endpoints will now include the token

## Configuration

Key configuration in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ProductApiDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ProductApi",
    "Audience": "ProductApiClients",
    "AccessTokenExpirationMinutes": "15",
    "RefreshTokenExpirationDays": "7"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

> **Note:** Never commit secrets. Use `appsettings.Development.json` (gitignored) or environment variables for local development.

## Deployment

### Build and Publish

```bash
dotnet publish src/API/ProductApi.API -c Release -o ./publish
```

### Docker Build

```bash
docker build -f src/API/ProductApi.API/Dockerfile -t product-api:latest .
docker run -p 5000:80 product-api:latest
```

### Docker Compose (production-like)

```bash
docker-compose up -d --build
```

### Environment Variables

For production, configure via environment variables:
- `ConnectionStrings__DefaultConnection` — SQL Server connection string
- `JwtSettings__SecretKey` — JWT signing key (min 32 characters)
- `JwtSettings__Issuer` — Token issuer
- `JwtSettings__Audience` — Token audience
- `ASPNETCORE_ENVIRONMENT` — Set to `Production` in production

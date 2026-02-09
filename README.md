# ICMarketWebAPI

ICMarketWebAPI is a **.NET 8 Web API** built using **Clean Architecture** principles.  
The project exposes endpoints for **synchronizing and querying blockchain snapshot data** using an external blockchain provider.

The solution is intentionally kept **API-only**, without authentication or UI layers, to focus on correctness, architecture, and testability.

---

## Tech Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
  - SQLite (runtime)
  - InMemory provider (unit tests)
- **MediatR** (CQRS)
- **AutoMapper** (query projection)
- **NSwag** (OpenAPI / Swagger UI)
- **FluentValidation**
- **xUnit / FluentAssertions**

---

## Architecture

The solution follows **Clean Architecture**:

```
src/
 ├─ Domain         // Entities, enums, domain logic
 ├─ Application    // Use cases (Commands / Queries), DTOs, Behaviours
 ├─ Infrastructure // EF Core, external services, persistence
 └─ Web            // API endpoints, middleware, OpenAPI
```

Key architectural decisions:

- CQRS via MediatR
- No authentication / authorization (not required by task)
- Minimal API endpoints grouped by feature
- Database migrations applied automatically in **Development**
- External dependencies isolated in Infrastructure layer

---

## Features

### Blockchain API

- **Sync blockchain snapshot**
- **Query blockchain history**
  - Filtered by network
  - Ordered by `CreatedAt DESC`
  - Configurable `take` limit with validation

Supported networks:

- `eth` / `ethmain`
- `btc` / `btcmain`
- `btctest3`
- `dash`
- `ltc`

---

## API Endpoints

### Sync blockchain snapshot

```
POST /api/blockchains/{chain}/sync
```

### Get blockchain history

```
GET /api/blockchains/{chain}/history?take=100
```

Example:

```
GET /api/blockchains/eth/history?take=50
```

---

## OpenAPI / Swagger

Swagger UI is available at:

```
https://localhost:5001/api
```

The OpenAPI specification is exposed at:

```
https://localhost:5001/api/specification.json
```

The root URL `/` automatically redirects to `/api`.

---

## Build

```
dotnet build
```

---

## Database

- SQLite is used for local development
- Database schema is created via EF Core migrations
- Migrations are applied automatically on startup in **Development** environment

---

## Testing

The solution contains:

- **Unit tests** (Application layer)
- **Functional tests** (API endpoints)

Run all tests:

```
dotnet test
```

Testing strategy:

- Unit tests validate query/command logic
- Functional tests spin up the full Web API using `WebApplicationFactory`
- SQLite InMemory is used to ensure isolated test runs

---

## Notes

- Authentication and authorization were intentionally omitted to keep the solution aligned with the task scope
- No UI or Razor Pages are included
- The project focuses on clean separation of concerns, readability, and testability

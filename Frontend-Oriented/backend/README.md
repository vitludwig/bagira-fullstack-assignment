# Scenario Builder - Backend API

This is the ASP.NET Core 8 API for the Scenario Builder application. It uses Entity Framework Core with PostgreSQL, Mapster for DTO mapping, and Gridify for server-side paging, filtering, and sorting.

## Setup

**Prerequisites:**
   - .NET 8.0 SDK or later
   - PostgreSQL 16 or a compatible PostgreSQL server
   - `dotnet-ef` tool when creating or inspecting migrations
   - Your preferred IDE (Visual Studio, Rider, VS Code)

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger` or `https://localhost:5001/swagger`

## Project Structure

```
backend/
├── Backend.Api/              # Web API project
│   ├── Controllers/          # API controllers
│   │   ├── ScenariosController.cs
│   │   └── EntitiesController.cs
│   ├── DTOs/                # Data Transfer Objects (temporary location)
│   │   ├── Scenario DTOs
│   │   └── Entity DTOs
│   └── Program.cs           # Application entry point
├── Backend.Domain/           # Domain layer
│   └── Models/              # Domain models and enums
│       ├── Scenario.cs
│       ├── Entity.cs
│       ├── EntityType.cs
│       └── TaskForce.cs
└── Backend.Infrastructure/  # Persistence layer
    ├── Data/                # EF Core context and entity configurations
    ├── Interfaces/          # Repository interfaces
    │   ├── IRepository.cs
    │   ├── IScenarioRepository.cs
    │   └── IEntityRepository.cs
    ├── Migrations/          # EF Core migrations
    └── Repositories/        # PostgreSQL repository implementations
        ├── ScenarioRepository.cs
        └── EntityRepository.cs
```

## API Endpoints

### Scenarios
- `GET /api/scenarios` - Get a paged, searchable, and sortable scenario list
- `GET /api/scenarios/{scenarioId}` - Get scenario by ID
- `POST /api/scenarios` - Create a new scenario

### Entities
- `GET /api/scenarios/{scenarioId}/entities` - Get a paged, searchable, filterable, and sortable entity list for a scenario
- `GET /api/entities/{entityId}` - Get entity by ID
- `POST /api/scenarios/{scenarioId}/entities` - Create a new entity

**Note:** The application supports Create and Read operations only. Update and Delete operations are bonus functionality and are not implemented.

## What's Implemented

- ✅ Create and Read API controllers
- ✅ DTOs for all requests and responses
- ✅ Domain models (Scenario, Entity) with enums (EntityType, TaskForce)
- ✅ Repository interfaces and EF Core implementations
- ✅ PostgreSQL persistence and referential integrity
- ✅ EF Core migrations applied during application startup
- ✅ Server-side paging, filtering, and sorting
- ✅ **DTO validation attributes** — request DTOs use the relevant `[Required]`, `[Range]`, `[MaxLength]`, and `[EnumDataType]` constraints
- ✅ **Automatic 400 responses** — invalid requests return `ErrorResponse` with field-level errors (no extra code needed)
- ✅ **Enum string deserialization** — frontend can send `"Soldier"`, `"Friendly"` etc. as strings
- ✅ Error response shape (ErrorResponse)
- ✅ Swagger/OpenAPI configuration
- ✅ Dependency injection setup for repositories

## Database

The development connection string is configured in `Backend.Api/appsettings.Development.json`. Docker overrides it through `ConnectionStrings__DefaultConnection`.

For local execution, create the configured database and role in PostgreSQL before starting the API. The default development connection expects PostgreSQL on `localhost:5432`, database `scenario_builder`, user `postgres`, and password `postgres`. These credentials are intended for local development only.

Install the EF Core CLI when it is not already available:

```bash
dotnet tool install --global dotnet-ef --version 8.*
```

Create a new migration with:

```bash
dotnet ef migrations add <MigrationName> \
  --project Backend.Infrastructure \
  --startup-project Backend.Api
```

## Key Requirements

- Entities must always belong to a Scenario (no orphan entities)
- Validate that a scenario exists before creating an entity under it (return `404` if it doesn't)
- Invalid request DTOs return a standardized `400` response with field-level validation errors

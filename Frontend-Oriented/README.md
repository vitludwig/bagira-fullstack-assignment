# Scenario Builder

Full-stack implementation of the Bagira Systems frontend-oriented assignment. The frontend uses Angular 21 and Angular Material. The backend uses ASP.NET Core 8, Entity Framework Core, PostgreSQL, Mapster, and Gridify.

## Run with Docker

### Prerequisites

- Docker with Docker Compose

### Start the application

From the `Frontend-Oriented` directory run:

```bash
docker compose up --build
```

The first run builds the images, initializes PostgreSQL, and applies pending EF Core migrations. For subsequent runs without source or dependency changes, use:

```bash
docker compose up
```

Run `docker compose up --build` again after changing application source code, dependencies, or a Dockerfile.

Open:

- Frontend: http://localhost:4200
- Backend API: http://localhost:5000

Stop the application with:

```bash
docker compose down
```

PostgreSQL data is stored in the `postgres_data` Docker volume and survives container restarts.

To remove the containers and all database data, run:

```bash
docker compose down --volumes
```

## Docker configuration

The following environment variables can be placed in a `.env` file next to `docker-compose.yml`:

| Variable | Default | Description |
| --- | --- | --- |
| `FRONTEND_PORT` | `4200` | Host port used by the Angular frontend |
| `POSTGRES_DB` | `scenario_builder` | PostgreSQL database name |
| `POSTGRES_USER` | `postgres` | PostgreSQL username |
| `POSTGRES_PASSWORD` | `postgres` | PostgreSQL password for local Docker use |

Copy `.env.example` to `.env` to customize the values.

`POSTGRES_DB`, `POSTGRES_USER`, and `POSTGRES_PASSWORD` initialize PostgreSQL only when the database volume is created for the first time. Changing them later does not update the existing database. For disposable local data, remove the volume with `docker compose down --volumes` before starting again with new values.

The frontend API URL is configured at build time in `frontend/frontend-angular/src/environments/environment.prod.ts`. The included Compose configuration and credentials are intended for local development only, not for production deployment.

## Run locally without Docker

### Backend

The local backend requires PostgreSQL on `localhost:5432`. Create the database and user referenced by `backend/Backend.Api/appsettings.Development.json`, or replace that development connection string with your local credentials. The default configuration expects database `scenario_builder` and user `postgres` with password `postgres`. EF Core applies pending migrations when the API starts.

```bash
cd backend
dotnet restore Backend.sln
dotnet run --project Backend.Api/Backend.Api.csproj --launch-profile http
```

The API runs at http://localhost:5000.

### Frontend

In another terminal:

```bash
cd frontend/frontend-angular
npm ci
npm start
```

The frontend runs at http://localhost:4200 and uses the API URL configured in `frontend/frontend-angular/src/environments/environment.ts`.

## Implemented

- Scenario list, creation, details, searching, sorting, and paging
- Entity creation, filtering, searching, sorting, paging, and table/map views
- Frontend and backend validation
- Loading, empty, and error states
- Material toast notifications for create operations
- Standardized API error responses and global exception handling
- CORS configuration
- Multi-stage Docker builds for backend and frontend
- PostgreSQL persistence with EF Core migrations

## Trade-offs

- Update/delete operations and global search are not implemented.
- The production frontend API URL is configured at build time through `frontend/frontend-angular/src/environments/environment.prod.ts`.
- The Docker Compose setup is intended for local single-instance use. Production deployment requires separate database roles, secret management, coordinated migrations, health checks, and backups.

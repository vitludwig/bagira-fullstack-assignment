# Scenario Builder

Full-stack implementation of the Bagira Systems frontend-oriented assignment. The frontend uses Angular 21 and Angular Material. The backend uses ASP.NET Core 8, Mapster, Gridify, and in-memory repositories.

## Run with Docker

### Prerequisites

- Docker with Docker Compose

### Start the application

From the `Frontend-Oriented` directory run:

```bash
docker compose up --build
```

Open:

- Frontend: http://localhost:4200
- Backend API: http://localhost:5000

Stop the application with:

```bash
docker compose down
```

The repositories are currently in-memory. Data is therefore cleared whenever the backend container restarts.

## Docker configuration

The following environment variables can be placed in a `.env` file next to `docker-compose.yml`:

| Variable | Default | Description |
| --- | --- | --- |
| `FRONTEND_PORT` | `4200` | Host port used by the Angular frontend |

Copy `.env.example` to `.env` to customize the values.

The frontend API URL is configured at build time in `src/environments/environment.prod.ts`.

## Run locally without Docker

### Backend

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

The frontend runs at http://localhost:4200 and uses the API URL configured in `src/environments/environment.ts`.

## Implemented

- Scenario list, creation, details, searching, sorting, and paging
- Entity creation, filtering, searching, sorting, and paging
- Frontend and backend validation
- Loading, empty, and error states
- Material toast notifications for create operations
- Standardized API error responses and global exception handling
- CORS configuration
- Multi-stage Docker builds for backend and frontend

## Trade-offs

- Repositories use in-memory storage, so data is not persisted between backend restarts.
- The map view, update/delete operations, and global search are not implemented.
- The production frontend API URL is configured at build time through the Angular environment file.

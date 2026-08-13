# NeoWatcher

NEO Watcher is a small ASP.NET Core application that syncs Near-Earth Object data from NASA and exposes an API and UI for daily aggregated statistics.

## Run locally

From the repository root:

```bash
dotnet build ./NeoWatcher/NeoWatcher.csproj
dotnet run --project ./NeoWatcher/NeoWatcher.csproj --urls http://localhost:5000
```

Open:

- API: http://localhost:5000/api/neo/stats
- Swagger/OpenAPI: http://localhost:5000/openapi
- UI: http://localhost:5000/Neo

## Tests

```bash
dotnet test ./NeoWatcher.Tests/NeoWatcher.Tests.csproj
```

## Notes

- In development the app uses an in-memory EF provider, so PostgreSQL is not required.
- For production, configure the `ConnectionStrings:NeoDb` setting and ensure migrations are applied.

## PostgreSQL with Docker Compose

```yaml
services:
  db:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: NeoWatcher
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres -d NeoWatcher"]
      interval: 5s
      timeout: 5s
      retries: 10

volumes:
  pgdata:
```

Start it with:

```bash
docker compose up -d
```

Example connection string:

```json
"ConnectionStrings": {
  "NeoDb": "Host=localhost;Port=5432;Database=NeoWatcher;Username=postgres;Password=postgres"
}
```

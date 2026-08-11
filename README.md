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
    image: postgres:15
    environment:
      POSTGRES_USER: neo
      POSTGRES_PASSWORD: neo
      POSTGRES_DB: neowatcher
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

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
  "NeoDb": "Host=localhost;Port=5432;Database=neowatcher;Username=neo;Password=neo"
}
```

When PostgreSQL is available, the app attempts to run migrations on startup. In development, the in-memory database is used by default.

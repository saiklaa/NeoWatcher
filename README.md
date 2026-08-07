# NeoWatcher

NEO Watcher — small ASP.NET Core app that syncs Near-Earth Object data from NASA and exposes an API and UI for daily aggregated stats.

How to run (Development):

```bash
dotnet build
dotnet run --project NeoWatcher --urls http://localhost:5000
```

Open:

- API: http://localhost:5000/api/neo/stats
- Swagger/OpenAPI: http://localhost:5000/openapi
- UI: http://localhost:5000/Neo

Tests:

```bash
dotnet test NeoWatcher/NeoWatcher.Tests
```

Notes:

- In Development the app uses an in-memory EF provider so you can run without PostgreSQL.
- For production configure `ConnectionStrings:NeoDb` and ensure migrations are applied.

Postgres (docker-compose)

```yaml
version: '3.8'
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

Start a Postgres instance via Docker Compose:

```bash
docker compose up -d
```

Example connection string (appsettings.json):

```json
"ConnectionStrings": {
	"NeoDb": "Host=localhost;Port=5432;Database=neowatcher;Username=neo;Password=neo"
}
```

When Postgres is available the app will attempt to run migrations on startup (Development uses InMemory DB by default).

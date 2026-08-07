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

Run migrations locally against Postgres (once DB is available):

```bash
dotnet ef migrations add Init -p NeoWatcher -s NeoWatcher
dotnet ef database update -p NeoWatcher -s NeoWatcher
```

Submission checklist:

- [ ] Build succeeds
- [ ] API endpoint `GET /api/neo/stats` implemented with filtering, grouping, sorting, caching
- [ ] Swagger docs present
- [ ] UI at `/Neo` renders stats table
- [ ] Unit & integration tests in `NeoWatcher.Tests`

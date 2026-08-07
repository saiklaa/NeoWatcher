SHELL := /bin/bash

.PHONY: up-db migrate start test

up-db:
	docker compose up -d db

migrate: up-db
	./scripts/migrate.sh

start: migrate
	ASPNETCORE_ENVIRONMENT=Production dotnet run --project NeoWatcher --urls http://localhost:5000

test:
	dotnet test NeoWatcher/NeoWatcher.Tests

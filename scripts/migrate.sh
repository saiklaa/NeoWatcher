#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

echo "Applying EF migrations to Postgres..."

# Use dotnet ef directly (no need for global tool when it's in csproj)
dotnet ef database update

echo "Migrations applied."

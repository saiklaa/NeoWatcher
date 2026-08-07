#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

echo "Applying EF migrations to Postgres..."

# Ensure dotnet-ef is available or use global tool
if ! command -v dotnet-ef >/dev/null 2>&1; then
  echo "dotnet-ef not found. Install it with: dotnet tool install --global dotnet-ef" >&2
  exit 1
fi

dotnet ef database update -p NeoWatcher -s NeoWatcher

echo "Migrations applied."

#!/usr/bin/env bash
set -euo pipefail

# Start postgres via docker compose and wait until it's ready
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

echo "Starting Postgres via docker compose..."
docker compose up -d db

echo "Waiting for Postgres to become available..."
TRIES=0
MAX_TRIES=30
while ! docker compose exec -T db pg_isready -U postgres -d NeoWatcher >/dev/null 2>&1; do
  TRIES=$((TRIES+1))
  if [ $TRIES -ge $MAX_TRIES ]; then
    echo "Postgres did not become ready in time" >&2
    exit 1
  fi
  sleep 2
done

echo "Postgres is ready."

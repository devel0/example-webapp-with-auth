#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/src/app/backend

dotnet ef migrations $@ \
    --context AppDbContext \
    --project ../../libs/db-migrations-psql \
    -- --provider Postgres

if [ "$1" == "" ]; then
    echo "example: $0 list"
fi
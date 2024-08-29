#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/src/app/backend

dotnet ef database $@ \
    --context AppDbcontext \
    --project ../../libs/db-migrations-psql \
    -- --provider Postgres

if [ "$1" == "" ]; then
    echo "example: $0 update"
fi
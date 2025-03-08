#!/bin/bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/src/backend/webapi

dotnet ef migrations $@ \
    --context AppDbContext \
    --project ../db-migrations-psql \
    -- --provider Postgres

if [ "$1" == "" ]; then
    echo "example: $0 list"
fi
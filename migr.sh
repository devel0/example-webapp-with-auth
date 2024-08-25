#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/WebApiServer

dotnet ef migrations $@ \
    --context AppDbContext \
    --project ../AppDbMigrationsPsql \
    -- --provider Postgres

if [ "$1" == "" ]; then
    echo "example: $0 list"
fi
#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$0"))

cd "$exdir"/WebApiServer

dotnet ef database $@ \
    --context AppDbContext \
    --project ../AppDbMigrationsPsql \
    -- --provider Postgres

if [ "$1" == "" ]; then
    echo "example: $0 update"
fi
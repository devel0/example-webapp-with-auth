#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

API_SPEC_URL="https://dev-webapp-test.searchathing.local/swagger/v1/swagger.json"

#--------

APIFLD=$exdir/src/app/frontend/api

rm -fr $APIFLD

mkdir $APIFLD

cd $exdir 

npx @openapitools/openapi-generator-cli generate \
    -i "$API_SPEC_URL" \
    -g typescript-axios \
    -o $APIFLD
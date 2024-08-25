#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

API_SPEC_URL="https://dev-webapp-test.searchathing.com/swagger/v1/swagger.json"

#--------

APIFLD=$exdir/clientapp/api

rm -fr $APIFLD

mkdir $APIFLD

cd $exdir 

npx @openapitools/openapi-generator-cli generate \
    -i "$API_SPEC_URL" \
    --skip-validate-spec \
    -g typescript-fetch \
    -o $APIFLD
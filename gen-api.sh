#!/usr/bin/env bash

function isUrlAvail() {
    q="$(curl $1 -o /dev/null -w '%{http_code}\n' -s)"
    if [ "$q" == "200" ]; then
        echo 1
    else
        echo 0
    fi
}

export NODE_OPTIONS=--use-openssl-ca

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

source "$exdir"/src/frontend/.env.development

app_api_spec_url="https://$VITE_SERVERNAME/swagger/v1/swagger.json"

#-------- app (typescript)

app_api_avail="$(isUrlAvail "$app_api_spec_url")"

if [ "$app_api_avail" == "1" ]; then

    APIFLD="$exdir"/src/frontend/api

    rm -fr "$APIFLD"

    mkdir "$APIFLD"

    cd "$exdir"

    npx @openapitools/openapi-generator-cli generate \
        -i "$app_api_spec_url" \
        -g typescript-axios \
        -o "$APIFLD"

    app_api_generated="1"

fi

#=================================================

if [ "$app_api_generated" != "1" ]; then echo "(W): skip typescript app api from $app_api_spec_url"; fi

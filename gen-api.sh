#!/usr/bin/env bash

export NODE_OPTIONS=--use-openssl-ca

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

source "$exdir"/src/app/frontend/.env.development

app_api_spec_url="https://$VITE_SERVERNAME/swagger/v1/swagger.json"

function isUrlAvail() {
    q="$(curl $1 -o /dev/null -w '%{http_code}\n' -s)"
    if [ "$q" == "200" ]; then
        echo 1
    else
        echo 0
    fi
}

app_api_avail="$(isUrlAvail "$app_api_spec_url")"

#--------

if [ "$app_api_avail" == "1" ]; then

    APIFLD=$exdir/src/app/frontend/api

    rm -fr $APIFLD

    mkdir $APIFLD

    cd $exdir

    npx @openapitools/openapi-generator-cli generate \
        -i "$app_api_spec_url" \
        -g typescript-axios \
        -o $APIFLD

    app_api_generated="1"

fi

if [ "$app_api_generated" != "1" ]; then

    echo "(W): skip typescript api from $app_api_spec_url"    

fi

#!/bin/bash

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

openapi-generator-cli version >&/dev/null

if [ "$?" != "0" ]; then
	pnpm i @openapitools/openapi-generator-cli -g
fi

if [ -e "$exdir/src/frontend/angular.json" ]; then
	# angular
	APIGENERATOR=typescript-angular
	APIFLD="$exdir"/src/frontend/src/api
	SRVHOSTNAME="$(cat "$exdir"/src/frontend/src/environments/environment.development.ts | grep basePath | sed -n "s#basePath: 'https://\(.*\)',#\1#p" | sed -e 's/^[[:space:]]*//' -e 's/[[:space:]]*$//')" 
	OPENAPI_FLAGS="--additional-properties serviceSuffix=ApiService"
else
	# react
	APIGENERATOR=typescript-axios
	APIFLD="$exdir"/src/frontend/api
	source "$exdir"/src/frontend/.env.development
	SRVHOSTNAME=$VITE_SERVERNAME
fi

app_api_spec_url="https://$SRVHOSTNAME/swagger/v1/swagger.json"

echo "SRVHOSTNAME=$SRVHOSTNAME"
echo "APIGENERATOR=$APIGENERATOR"
echo "spec url $app_api_spec_url"

#-------- app (typescript)

app_api_avail="$(isUrlAvail "$app_api_spec_url")"

if [ "$app_api_avail" == "1" ]; then

	rm -fr "$APIFLD"

	mkdir "$APIFLD"

	cd "$exdir"

	openapi-generator-cli generate \
		-i "$app_api_spec_url" \
		-g $APIGENERATOR \
		$OPENAPI_FLAGS \
		-o "$APIFLD"

	app_api_generated="1"

fi

#=================================================

if [ "$app_api_generated" != "1" ]; then echo "(W): skip typescript app api from $app_api_spec_url"; fi

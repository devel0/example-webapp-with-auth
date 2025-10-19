#!/bin/sh

if [ "$APP_SERVERNAME" = "" ]; then
	echo "missing APP_SERVERNAME var"
	exit 10
fi

if [ "$FRONTEND_SRCDIR" = "" ]; then
	echo "missing FRONTEND_SRCDIR var"
	exit 10
fi

exdir=$(dirname "$(readlink -f "$BASH_SOURCE")")

GITCOMMIT="$(git rev-parse --short HEAD)"
GITCOMMITDATE="$(git show -s --format="%ci" $GITCOMMIT)"

# frontend env

ENVFILE="$FRONTEND_SRCDIR"/src/environments/environment.production.ts

sed -i "s#basePath:.*#basePath: \"https://$APP_SERVERNAME\",#g" "$ENVFILE"
sed -i "s#ssoPath:.*#ssoPath: \"https://$APP_SERVERNAME/app/login\",#g" "$ENVFILE"
sed -i "s#commit:.*#commit: \"$GITCOMMIT\",#g" "$ENVFILE"
sed -i "s#commitDate:.*#commitDate: \"$GITCOMMITDATE\",#g" "$ENVFILE"

exit 0
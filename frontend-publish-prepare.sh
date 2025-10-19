#!/bin/sh

if [ "$APP_SERVERNAME" == "" ]; then
	echo "missing APP_SERVERNAME var"
	exit 10
fi

if [ "$FRONTEND_SRCDIR" == "" ]; then
	echo "missing FRONTEND_SRCDIR var"
	exit 10
fi

exdir=$(dirname "$(readlink -f "$BASH_SOURCE")")

GITCOMMIT="$(git rev-parse --short HEAD)"
GITCOMMITDATE="$(git show -s --format="%ci" $GITCOMMIT)"

# frontend env

sed -i "s/VITE_SERVERNAME=.*/VITE_SERVERNAME=$APP_SERVERNAME/g" "$FRONTEND_SRCDIR"/.env.production
sed -i "s/VITE_GITCOMMIT=.*/VITE_GITCOMMIT=$GITCOMMIT/g" "$FRONTEND_SRCDIR"/.env.production
sed -i "s/VITE_GITCOMMITDATE=.*/VITE_GITCOMMITDATE=\"$GITCOMMITDATE\"/g" "$FRONTEND_SRCDIR"/.env.production

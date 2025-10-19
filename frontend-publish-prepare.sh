#!/bin/sh

exdir=$(dirname "$(readlink -f "$BASH_SOURCE")")

echo "APP_SERVERNAME=$APP_SERVERNAME"
echo "FRONTEND_SRCDIR=$FRONTEND_SRCDIR"

BACKEND_SRCDIR="$exdir"/src/backend

GITCOMMIT="$(git rev-parse --short HEAD)"
GITCOMMITDATE="$(git show -s --format="%ci" $GITCOMMIT)"

# backend appsettings
PRODFILE="$BACKEND_SRCDIR"/appsettings.Production.json
PRODFILE_TMP="$BACKEND_SRCDIR"/appsettings.Production.json.tmp
cat "$PRODFILE" |
    jq ".GitCommit |= \"$GITCOMMIT\"" |
    jq ".GitCommitDate |= \"$GITCOMMITDATE\"" |
    jq ".AppServerName |= \"$APP_SERVERNAME\"" \
        >"$PRODFILE_TMP"
mv -f "$PRODFILE_TMP" "$PRODFILE"

# frontend vite env

sed -i "s/VITE_SERVERNAME=.*/VITE_SERVERNAME=$APP_SERVERNAME/g" "$FRONTEND_SRCDIR"/.env.production
sed -i "s/VITE_GITCOMMIT=.*/VITE_GITCOMMIT=$GITCOMMIT/g" "$FRONTEND_SRCDIR"/.env.production
sed -i "s/VITE_GITCOMMITDATE=.*/VITE_GITCOMMITDATE=\"$GITCOMMITDATE\"/g" "$FRONTEND_SRCDIR"/.env.production

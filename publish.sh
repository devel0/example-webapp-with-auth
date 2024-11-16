#!/usr/bin/env bash

# set -x

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

FORCE="0"
BACKEND_SRCDIR="$exdir"/src/backend
FRONTEND_SRCDIR="$exdir"/src/frontend
APP_SSH_HOST=""
APP_SEVERNAME=""
APP_ID=""

printHelp() {
    echo "$0 argmuments:"
    echo "  -h <sshhost>        ssh host where to publish ( ie. main-test )"
    echo "  -sn <servername>    nginx app servername ( ie. mytest.searchathing.local )"
    echo "  -id <appid>         app identifier ( ie. mytest )"    
    echo "  -f                  force overwrite existing"
}

mandatoryArg() {
    if [ "$1" == "" ]; then
        printHelp
        exit 1
    fi
}

header() {
    echo "========================================================================="
    echo " $1"
    echo "========================================================================="
}

while [ "$1" != "" ]; do

    if [ "$1" == "--help" ]; then
        printHelp
        exit 1
    fi

    if [ "$1" == "-f" ]; then
        FORCE="1"
        shift
        continue
    fi

    if [ "$1" == "-h" ]; then
        shift
        mandatoryArg "$1"
        APP_SSH_HOST=$1
        shift
        continue
    fi

    if [ "$1" == "-sn" ]; then
        shift
        mandatoryArg "$1"
        APP_SERVERNAME=$1
        shift
        continue
    fi

    if [ "$1" == "-id" ]; then
        shift
        mandatoryArg "$1"
        APP_ID=$1
        shift
        continue
    fi

    if [ "$1" != "" ]; then
        echo "Unknown arg $1"
        printHelp
        exit 1
    fi

done

if [ "$APP_SSH_HOST" == "" ] || [ "$APP_SERVERNAME" == "" ] || [ "$APP_ID" == "" ]; then
    mandatoryArg
fi

cd "$exdir"

ssh $APP_SSH_HOST exit 100
testssh=$?

if [ "$testssh" != "100" ]; then
    echo "unable to connect ssh host $APP_SSH_HOST"

    exit 2
fi

# ======================================================================
header "PROD ENV"
# ======================================================================

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

# ======================================================================
header "PREPARE $APP_ID"
# ======================================================================

REMOTE_DEPLOY=/root/deploy/$APP_ID
ssh $APP_SSH_HOST mkdir -p "$REMOTE_DEPLOY"
rsync -arvx --delete deploy/ $APP_SSH_HOST:$REMOTE_DEPLOY/

forceargs=""

if [ "$FORCE" == "1" ]; then forceargs="-f"; fi

ssh $APP_SSH_HOST "$REMOTE_DEPLOY/scripts/prepare.sh" -sn "$APP_SERVERNAME" -id "$APP_ID" "$forceargs"
excode="$?"
if [ "$excode" == "10" ]; then
    echo "some prerequisites missing"
    exit 10
fi

# ======================================================================
header "BUILDING $APP_ID SERVER"
# ======================================================================

dotnet publish -c Release --runtime linux-x64 --sc
PREBUILT="$BACKEND_SRCDIR/bin/Release/net8.0/linux-x64/publish"

ssh $APP_SSH_HOST mkdir -p /srv/app/$APP_ID
rsync -arx --delete $PREBUILT/ $APP_SSH_HOST:/srv/app/$APP_ID/bin/

# ======================================================================
header "RESTARTING $APP_ID SERVER"
# ======================================================================

ssh $APP_SSH_HOST "service $APP_ID-webapp restart"

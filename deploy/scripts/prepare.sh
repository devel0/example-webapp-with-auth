#!/usr/bin/env bash

source /etc/environment

FORCE=false
APP_SEVERNAME=""
APP_ID=""

printHelp() {    
    echo "$0 argmuments:"    
    echo "  -id <appid>         app identifier ( ie. mytest )"    
    echo "  -sn <servername>    nginx app servername ( ie. mytest.searchathing.com )"
    echo "  -f                  force overwrite existing"
}

mandatoryArg() {
    if [ "$1" == "" ]; then
        printHelp
        exit 10
    fi
}

while [ "$1" != "" ]; do

    if [ "$1" == "--help" ]; then
        printHelp
        exit 1
    fi    

    if [ "$1" == "-f" ]; then
        FORCE=true
        shift
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
        mandatoryArg  "$1"
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

if [ "$APP_SERVERNAME" == "" ] || [ "$APP_ID" == "" ]; then
    mandatoryArg
fi

#--- services

SERVICES=(
    webapp.service    
)

for i in ${SERVICES[@]}; do

    if $force || [ ! -e /etc/systemd/system/$APP_ID-$i ]; then

        cp -v /root/deploy/$APP_ID/service/$i /etc/systemd/system/$APP_ID-$i

        systemctl enable $APP_ID-$i

    fi

done

#--- nginx

if [ ! -e /etc/nginx/conf.d ]; then
    echo "nginx missing ( solve with apt install nginx )"
    exit 10
fi

NGINX_FILES=(
    webapp.conf
)

for i in ${NGINX_FILES[@]}; do

    RESTART_NGINX=false

    if $force || [ ! -e /etc/nginx/conf.d/$APP_ID-$i ]; then

        DSTCONF="/etc/nginx/conf.d/$APP_ID-$i"

        cp -v /root/deploy/$APP_ID/nginx/prod/$i "$DSTCONF"

        sed -i "s/\stest\.searchathing\.com/ $APP_SERVERNAME/g" "$DSTCONF"
        sed -i "s/webapp-test\.access/$APP_ID.access/g" "$DSTCONF"
        sed -i "s/webapp-test\.error/$APP_ID.error/g" "$DSTCONF"
        sed -i "/#>/d" "$DSTCONF"

        RESTART_NGINX=true

    fi

    if $RESTART_NGINX; then service nginx restart; fi

done

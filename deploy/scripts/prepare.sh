#!/usr/bin/env bash

source /etc/environment

FORCE=false
APP_ID=""

printHelp() {    
    echo "argmuments:"    
    echo "  -id <appid>         app identifier ( ie. webapp-test )"    
    echo "  -f                  force overwrite existing"
}

mandatoryArg() {
    if [ "$1" == "" ]; then
        printHelp
        exit 1
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

#--- services

SERVICES=(
    $APP_ID.service    
)

for i in ${SERVICES[@]}; do

    if $force || [ ! -e /etc/systemd/system/$i ]; then

        cp -v /root/deploy/$APP_ID/service/$i /etc/systemd/system

        systemctl enable $i

    fi

done

#--- nginx

if [ ! -e /etc/nginx/conf.d ]; then
    echo "nginx missing ( solve with apt install nginx )"
    exit 10
fi

NGINX_FILES=(
    $APP_ID.conf
)

for i in ${NGINX_FILES[@]}; do

    RESTART_NGINX=false

    if $force || [ ! -e /etc/nginx/conf.d/$i ]; then

        cp -v /root/deploy/$APP_ID/nginx/prod/$i /etc/nginx/conf.d

        RESTART_NGINX=true

    fi

    if $RESTART_NGINX; then service nginx restart; fi

done

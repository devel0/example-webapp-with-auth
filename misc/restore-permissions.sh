#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/..

chmod -v +x \
    db.sh \
    migr.sh \
    gen-api.sh \
    publish.sh \
    run-frontend.sh \
    deploy/scripts/prepare.sh \
    doc/gen-db-dia.sh

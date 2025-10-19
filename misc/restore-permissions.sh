#!/bin/bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/..

chmod -v +x \
    db.sh \
    migr.sh \
    gen-api.sh \
    publish.sh \
    run-frontend.sh \
	frontend-publish-prepare.sh \
    deploy/scripts/prepare.sh \
    misc/gen-db-dia.sh

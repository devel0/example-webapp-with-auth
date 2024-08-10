#!/bin/bash

# ref: https://github.com/devel0/knowledge/blob/master/doc/psql-schema-crawler.md

exdir="$(dirname "$(readlink -f "$0")")"

DST="$exdir"/doc/db
DBHOST=localhost
DBNAME=ExampleWebApp

if [ ! -e "$DST" ]; then
    echo "expecting destination folder [$DST]"
    exit 0
fi

cd "$DST"

SC_GRAPHVIZ_OPTS='-Granksep=2' ~/opt/schemacrawler/bin/schemacrawler.sh \
    --log-level=INFO \
    --portable-names \
    --sort-columns \
    --table-types=TABLE \
    --server=postgresql \
    --command=schema \
    --host=$DBHOST \
    --user=postgres \
    --password="$(cat ~/security/psql)" \
    --database=$DBNAME \
    -o=db.png \
    --outputformat=png \
    --info-level=standard

cd "$exdir"

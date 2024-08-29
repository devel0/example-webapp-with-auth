#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/src/app/frontend
npm run dev

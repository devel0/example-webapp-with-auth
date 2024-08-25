#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/clientapp
npm run dev

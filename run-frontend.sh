#!/usr/bin/env bash

exdir="$(dirname "$(readlink -f "$0")")"

cd "$exdir"/clientapp
npm run dev

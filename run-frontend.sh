#!/bin/bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"/src/frontend
pnpm i
pnpm run dev

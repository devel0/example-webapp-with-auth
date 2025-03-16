#!/bin/bash

exdir=$(dirname $(readlink -f "$BASH_SOURCE"))

cd "$exdir"

export AppConfig__IsUnitTest=true

dotnet test $@

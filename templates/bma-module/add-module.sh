#!/usr/bin/env bash
set -euo pipefail
dotnet sln "$1" add "$2"
dotnet add "$3" reference "$2"

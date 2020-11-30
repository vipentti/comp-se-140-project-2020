#!/bin/sh

file="docker-compose.yml"

docker-compose -f "${file}" config | egrep -A10 "^\s+$1" | grep "TargetProject" | sed 's/.*TargetProject: //g'

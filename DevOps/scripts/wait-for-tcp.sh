#!/bin/sh

# wait for a tcp connection
# NOTE: this does not guarantee that the service is actually working
# Just that the port that is used can be connected to.

set -e

attempt=0
max_attempts=30

host="$1"
port="$2"
shift 3
cmd="$@"

>&2 echo "Waiting for ${host}:${port} to respond to TCP."

until nc -z "${host}" "${port}"; do
    attempt=$((attempt+1))
    if [ $attempt -gt $max_attempts ] ; then
        >&2 echo "${host}:${port} is not up after $attempt attempt(s). Exiting."
        exit 1
    fi
    sleep 1
done

>&2 echo "${host}:${port} is up after $attempt attempt(s)"

exec $cmd

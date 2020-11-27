#!/bin/sh

# Based on
# https://docs.docker.com/compose/startup-order/

set -e

container_name="${1:?ContainerNameRequired}"
log_message="${2:?logmessage}"


attempt=0
max_attempts=30

until docker logs --tail 50 "${container_name}" 2>&1 | grep -q "${log_message}" ; do
    ((attempt=attempt+1))
    if [ $attempt -gt $max_attempts ] ; then
        >&2 echo "Container '${container_name}' is not up after $attempt attempt(s). Exiting."
        exit 1
    fi

    >&2 echo "Waiting for '${container_name}' logs to contain '${log_message}'"
    sleep 1
done

>&2 echo "'${container_name}' is up"

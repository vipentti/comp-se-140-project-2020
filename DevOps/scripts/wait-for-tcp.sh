#!/bin/sh

# wait for a tcp connection
# NOTE: this does not guarantee that the service is actually working
# Just that the port that is used can be connected to.

attempt=0
max_attempts=30

host="$1"
port="$2"
shift 3
cmd="$@"

>&2 echo "Waiting for ${host}:${port} to respond to TCP."

tcp_wait() {
    res=$(timeout 5 bash -c "cat < /dev/null > /dev/tcp/${host}/${port}" 2>&1 > /dev/null)
    return $?
}

# until nc -z "${host}" "${port}"; do
## until timeout 5 bash -c "cat < /dev/null > /dev/tcp/${host}/${port}" && $?; do
until tcp_wait ; do
    attempt=$((attempt+1))
    if [ $attempt -gt $max_attempts ] ; then
        >&2 echo "${host}:${port} is not up after $attempt attempt(s). Exiting."
        exit 1
    fi
    sleep 1
done

# timeout 30 bash -c "cat < /dev/null > /dev/tcp/${host}/${port}"; 

if [ ! $? -eq 0 ]; then
    >&2 echo "${host}:${port} is not up after $attempt attempt(s). Exiting."
    exit 1;
fi

>&2 echo "${host}:${port} is up after $attempt attempt(s)"

exec $cmd

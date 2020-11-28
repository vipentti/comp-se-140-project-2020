#!/bin/sh

Dockerfile="${1:?Dockerfile}"
BuildContext="${2?:BuildContext}"
EnvFile="${3?:EnvFile}"
TestResultsPath="${4:-$(pwd)/testresults}"
Tag="${Tag:-latest}"
ImageAndTag="${5:-builder:latest}"

script_dir=$(dirname $(readlink -f $0))

/bin/sh "${script_dir}/build-docker.sh" \
    --file "Dockerfile" \
    --tag "builder:${Tag}" \
    --target "testrunner"

if [ ! $? -eq 0 ]; then
    echo "Building containers failed. Exiting."
    exit 1;
fi

CONTAINER_NAME=amqptestrunner

CONTAINER_ID=$(docker ps -aqf "name=^${CONTAINER_NAME}$")

# Cleanup old containers
if [ -z "${CONTAINER_ID}" ] ; then
    echo "No container found"
else
    docker rm "${CONTAINER_ID}"
fi

if [ ! -f "${EnvFile}" ] ; then
    echo "env-file ${EnvFile} does not exist. Creating an empty file."
    touch "${EnvFile}"
else
    echo "Using env-file ${EnvFile}"
    cat "${EnvFile}"
    echo ""
fi

echo "Starting test run..."
echo ""

docker run \
    --name "${CONTAINER_NAME}" ${EXTRA_RUN_ARGS:-} \
    --env-file "${EnvFile}" \
    $ImageAndTag

CODE=$?

echo "Test run returned '${CODE}'"

echo "Writing testresults to $TestResultsPath"
# Having to resort to copying from the container
# due to problems with mounting volumes when running
# in gitlab ci using docker in docker
# https://gitlab.com/gitlab-org/gitlab-foss/-/issues/41227
docker cp "${CONTAINER_NAME}":/src/testresults/. "${TestResultsPath}"

CONTAINER_ID=$(docker ps -aqf "name=^${CONTAINER_NAME}$")

# Cleanup containers
if [ -z "${CONTAINER_ID}" ];
then
    echo "No container found"
else
    docker rm "${CONTAINER_ID}"
fi

# We need to propagate the exit code from the tests
# to ensure that the test-jobs fail if any tests fail
exit $CODE

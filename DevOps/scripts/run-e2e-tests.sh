#!/bin/sh

E2E_PROJECT_NAME="${E2E_PROJECT_NAME:-e2e-test}"
ENV_FILE="${ENV_FILE:-.env.e2e}"
BUILD_CONFIG="${BUILD_CONFIG:-Release}"

docker-compose --project-name "${E2E_PROJECT_NAME}" down --volumes

script_dir=$(dirname $(readlink -f $0))

/bin/sh "${script_dir}/build-images.sh" --build-config "${BUILD_CONFIG}"

if [ ! $? -eq 0 ]; then
    echo "Building containers failed. Exiting."
    exit 1;
fi

docker-compose --project-name "${E2E_PROJECT_NAME}" up -d --no-build

if [ ! $? -eq 0 ]; then
    echo "Starting containers failed. Exiting."
    exit 1;
fi

export EXTRA_RUN_ARGS="--network ${E2E_PROJECT_NAME}_backend"

mkdir -pv ./testresults/e2e

/bin/sh "${script_dir}"/run-docker-tests.sh ./Dockerfile . ${ENV_FILE} ./testresults/e2e

TEST_EXIT_CODE=$?

echo "Tests returned $TEST_EXIT_CODE"

docker-compose --project-name "${E2E_PROJECT_NAME}" down --volumes

exit $TEST_EXIT_CODE

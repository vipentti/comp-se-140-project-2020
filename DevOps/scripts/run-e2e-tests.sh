#!/bin/sh

E2E_PROJECT_NAME="${E2E_PROJECT_NAME:-e2e-test}"
ENV_FILE="${ENV_FILE:-.env.e2e}"
BUILD_CONFIG="${BUILD_CONFIG:-Release}"

TEST_RESULTS_PATH="${TEST_RESULTS_PATH:-./testresults}"
script_dir=$(dirname $(readlink -f $0))

# Ensure env file sets E2E properly
export E2E="true"

if [ ! -f "${ENV_FILE}" ] ; then
    echo "env-file ${ENV_FILE} does not exist. Creating..."
    /bin/sh "${script_dir}/create-test-env-file.sh" "${ENV_FILE}"

    if [ ! $? -eq 0 ]; then
        echo "Failed to create env-file"
        exit 1;
    fi
fi

echo "Using env-file ${ENV_FILE}"
cat "${ENV_FILE}"
echo ""

docker-compose --project-name "${E2E_PROJECT_NAME}" --env-file "${ENV_FILE}" down --volumes

/bin/sh "${script_dir}/build-images.sh" --build-config "${BUILD_CONFIG}"

if [ ! $? -eq 0 ]; then
    echo "Building containers failed. Exiting."
    exit 1;
fi


docker-compose --project-name "${E2E_PROJECT_NAME}" --env-file "${ENV_FILE}" up -d --no-build

if [ ! $? -eq 0 ]; then
    echo "Starting containers failed. Exiting."
    exit 1;
fi

export EXTRA_RUN_ARGS="--network ${E2E_PROJECT_NAME}_backend"

mkdir -pv "${TEST_RESULTS_PATH}"

/bin/sh "${script_dir}"/run-docker-tests.sh ./Dockerfile . ${ENV_FILE} "${TEST_RESULTS_PATH}"

TEST_EXIT_CODE=$?

echo "Tests returned $TEST_EXIT_CODE"

if [ ! $TEST_EXIT_CODE -eq 0 ]; then
    echo "Dumping logs"
    docker-compose --project-name "${E2E_PROJECT_NAME}" logs --no-color > "${TEST_RESULTS_PATH}"/service-logs.txt 2>&1
fi

docker-compose --project-name "${E2E_PROJECT_NAME}" --env-file "${ENV_FILE}" down --volumes

exit $TEST_EXIT_CODE

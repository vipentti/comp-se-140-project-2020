#!/bin/sh

E2E_PROJECT_NAME=e2e-test
ENV_FILE="${ENV_FILE:-.env.e2e}"


docker-compose --project-name "${E2E_PROJECT_NAME}" down

docker-compose --project-name "${E2E_PROJECT_NAME}" up -d --build

if [ ! $? -eq 0 ]; then
    echo "Starting containers failed. Exiting."
    exit 1;
fi


export EXTRA_RUN_ARGS="--network ${E2E_PROJECT_NAME}_backend"

./DevOps/scripts/run-docker-tests.sh ./Dockerfile . ${ENV_FILE} ./testresults/e2e

TEST_EXIT_CODE=$?

echo "Tests returned $TEST_EXIT_CODE"

docker-compose --project-name "${E2E_PROJECT_NAME}" down

exit $TEST_EXIT_CODE

#!/bin/sh

E2E_PROJECT_NAME="${E2E_PROJECT_NAME:-e2e-test}"
ENV_FILE="${ENV_FILE:-.env.e2e}"

docker-compose --project-name "${E2E_PROJECT_NAME}" down

docker-compose --project-name "${E2E_PROJECT_NAME}" build

if [ ! $? -eq 0 ]; then
    echo "Building containers failed. Exiting."
    exit 1;
fi

# Start rabbitmq before everything else
docker-compose --project-name "${E2E_PROJECT_NAME}" up -d --no-deps rabbitmq

rabbitmq_container=$(docker inspect -f "{{.Name}}" $(docker-compose --project-name "${E2E_PROJECT_NAME}" ps -q rabbitmq) | cut -c2-)

script_dir=$(dirname $(readlink -f $0))

/bin/sh "${script_dir}/wait-for-docker-log.sh" "${rabbitmq_container}" "started TCP listener on"

docker-compose --project-name "${E2E_PROJECT_NAME}" up -d --no-build

if [ ! $? -eq 0 ]; then
    echo "Starting containers failed. Exiting."
    exit 1;
fi

export EXTRA_RUN_ARGS="--network ${E2E_PROJECT_NAME}_backend"

mkdir -pv ./testresults/e2e

./DevOps/scripts/run-docker-tests.sh ./Dockerfile . ${ENV_FILE} ./testresults/e2e

TEST_EXIT_CODE=$?

echo "Tests returned $TEST_EXIT_CODE"

docker-compose --project-name "${E2E_PROJECT_NAME}" down

exit $TEST_EXIT_CODE

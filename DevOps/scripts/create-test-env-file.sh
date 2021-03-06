#!/bin/sh

envfile="${1:?PathRequired}"

cat << EOF > "${envfile}"
TEST_FILTER=${TEST_FILTER}
FailingTest=${FailingTest}
EXTRA_RUN_ARGS=${EXTRA_RUN_ARGS}
E2E=${E2E}
HTTP_SERVER_PORT=${HTTP_SERVER_PORT:-9512}
APIGATEWAY_PORT=${APIGATEWAY_PORT:-9513}
RABBITMQ_PUBLIC_PORT=${RABBITMQ_PUBLIC_PORT:-5777}
RABBITMQ_MANAGEMENT_PUBLIC_PORT=${RABBITMQ_MANAGEMENT_PUBLIC_PORT:-15677}
REDIS_PUBLIC_PORT=${REDIS_PUBLIC_PORT:-16379}
EOF

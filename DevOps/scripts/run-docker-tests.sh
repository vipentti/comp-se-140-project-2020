#!/bin/sh

# set -o errexit
# set -o pipefail
# set -o nounset

localdir=$(pwd)
Dockerfile="${1}"
BuildContext="${2:-.}"
TestResultsPath="${3:-./testresults}"
Tag="${4:-amqptest:latest}"

docker build \
    --target "testrunner" \
    -t $Tag \
    -f $Dockerfile \
    $BuildContext

docker run \
    --rm \
    --network host \
    --env "TEST_FILTER=FullyQualifiedName~E2E" \
    --volume "$localdir/testresults:/src/testresults" \
    $Tag

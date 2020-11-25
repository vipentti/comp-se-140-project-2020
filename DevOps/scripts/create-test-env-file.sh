#!/bin/sh

envfile="${1:?PathRequired}"

touch "${envfile}"
echo "TEST_FILTER=${TEST_FILTER}" >> "${envfile}"
echo "HttpServerUrl=${HttpServerUrl}" >> "${envfile}"
echo "FailingTest=${FailingTest:-}" >> "${envfile}"

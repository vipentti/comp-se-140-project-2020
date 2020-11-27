#!/bin/sh

set -e

usage()
{
    cat <<EOF
usage: Test
EOF
}

# if [ "$1" = "" ]; then
#     usage
#     exit 1
# fi

Target="build"
tag="latest"
CacheImageAndTag=""
Dockerfile="Dockerfile"
BuildContext="."

while [ "$1" != "" ]; do
    case $1 in
        -f | --file )           shift
                                filename="$1"
                                ;;
        -i | --interactive )    interactive=1
                                ;;

        -t | --tag )            shift
                                tag="$1"
                                ;;
        -g | --target )         shift
                                Target="$1"
                                ;;
        -h | --help )           usage
                                exit
                                ;;
        * )                     usage
                                exit 1
                                ;;
    esac
    shift
done


script_dir=$(dirname $(readlink -f $0))

/bin/sh "${script_dir}/build-docker.sh" \
    --file "Dockerfile" \
    --tag "builder:${tag}" \
    --target "testrunner"

TargetProject=""

docker-compose config --services | while read line ; do
    echo === $line ===

    if [ "${line}" != "builder" ] && [  "${line}" != "rabbitmq" ]; then

        TargetProject=$(
            "${script_dir}/get-target-project.sh" "${line}"
        )

        /bin/sh "${script_dir}/build-docker.sh" \
                --file "Dockerfile" \
                --tag "${line}:${tag}" \
                --cache-from "builder:${tag}" \
                --build-args "--build-arg TargetProject=${TargetProject}" \
                --target ""
    fi

done

docker-compose config --services | while read line ; do
    if [ "${line}" != "builder" ] && [  "${line}" != "rabbitmq" ]; then
        TargetProject=$(
            "${script_dir}/get-target-project.sh" "${line}"
        )
        echo === Built $line $TargetProject ===
    fi
done

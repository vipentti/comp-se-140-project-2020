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
BuildConfig="Release"

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
        -bc | --build-config )  shift
                                BuildConfig="$1"
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


echo "Building in ${BuildConfig}-mode"

script_dir=$(dirname $(readlink -f $0))

/bin/sh "${script_dir}/build-docker.sh" \
    --file "Dockerfile" \
    --tag "builder:${tag}" \
    --build-args "--build-arg BUILD_CONFIG=${BuildConfig}" \
    --target "testrunner"

TargetProject=""

docker-compose config --services | while read line ; do
    echo === $line ===

    TargetProject="$(
        "${script_dir}/get-target-project.sh" "${line}"
    )"

    if [  "${TargetProject}" != "" ] ; then

        /bin/sh "${script_dir}/build-docker.sh" \
                --file "Dockerfile" \
                --tag "${line}:${tag}" \
                --cache-from "builder:${tag}" \
                --build-args "--build-arg TargetProject=${TargetProject} --build-arg BUILD_CONFIG=${BuildConfig}" \
                --target ""
    fi

done

docker-compose config --services | while read line ; do
    TargetProject="$(
        "${script_dir}/get-target-project.sh" "${line}"
    )"
    if [ "${targetProject}" != "" ] ; then
        echo === Built $line $TargetProject ===
    fi
done

#!/bin/sh

set -e

usage()
{
    cat <<EOF
usage: Test

EOF
}

if [ "$1" = "" ]; then
    usage
    exit 1
fi

Target=""
ImageAndTag=""
CacheImageAndTag=""
Dockerfile="Dockerfile"
BuildContext="."
BuildArgs=""

while [ "$1" != "" ]; do
    case $1 in
        -f | --file )           shift
                                Dockerfile="$1"
                                ;;
        -i | --interactive )    interactive=1
                                ;;

        -t | --tag )            shift
                                ImageAndTag="$1"
                                ;;

        -c | --cache-from )     shift
                                CacheImageAndTag="$1"
                                ;;

        -t | --tag )            shift
                                ImageAndTag="$1"
                                ;;
        -a | --build-args )     shift
                                BuildArgs="$1"
                                ;;
        -g | --target )         shift
                                Target="$1"
                                ;;
        -h | --help )           usage
                                exit
                                ;;
        * )                     usage
                                exit 1
    esac
    shift
done

targetArg=""
cacheFrom=""

if [ "${Target}" != "" ] ; then
    targetArg="--target ${Target}"
fi

if [ "${CacheImageAndTag}" != "" ] ; then
    cacheFrom="--cache-from ${CacheImageAndTag}"
fi

docker build \
    $targetArg \
    $cacheFrom \
    $BuildArgs \
    -t "${ImageAndTag}" \
    -f "${Dockerfile}" \
    "${BuildContext}"

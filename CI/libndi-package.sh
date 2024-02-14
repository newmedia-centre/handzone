#!/bin/bash

#
# This script calls libndi-get.sh and then libndi-create-deb.sh and libndi-create-dev-deb.sh
#

set -e

LIBNDI_VERSION="5.6.0"

SCRIPT_DIR=$(dirname "$0")

pushd $SCRIPT_DIR

./libndi-get.sh

GIT_HASH=$(git rev-parse --short HEAD)
PKG_VERSION="1-$GIT_HASH-$BRANCH_SHORT_NAME-git"

if [[ $BRANCH_FULL_NAME =~ ^refs\/tags\/ ]]; then
	PKG_VERSION="$BRANCH_SHORT_NAME"
fi

echo GIT_HASH=$GIT_HASH
echo PKG_VERSION=$PKG_VERSION

pushd ../build_x86_64

PAGER="cat" sudo checkinstall -y --type=debian --fstrans=no --nodoc \
    --backup=no --deldoc=yes --install=no \
    --pkgname=libndi5 --pkgversion="$LIBNDI_VERSION" \
    --replaces="libndi4, libndi3" \
    --pkglicense="Proprietary" \
    --maintainer="stephane.lepin@gmail.com" \
    --pkggroup="video" \
    --pkgsource="https://downloads.ndi.tv" \
    --pakdir="../package" ../CI/libndi-create-deb.sh

PAGER="cat" sudo checkinstall -y --type=debian --fstrans=no --nodoc \
    --backup=no --deldoc=yes --install=no \
    --pkgname=libndi5-dev --pkgversion="$LIBNDI_VERSION" \
    --replaces="libndi4-dev, libndi3-dev" \
    --requires="libndi5 \(\>= ${LIBNDI_VERSION}\)" \
    --pkglicense="Proprietary" --maintainer="stephane.lepin@gmail.com" \
    --pkggroup="video" \
    --pkgsource="https://downloads.ndi.tv" \
    --pakdir="../package" ../CI/libndi-create-dev-deb.sh

sudo chmod ao+r ../package/*

popd

popd

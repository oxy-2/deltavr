#!/usr/bin/env sh

# Copyright (c) 2022-2024 Antmicro <www.antmicro.com>
#
# SPDX-License-Identifier: Apache-2.0
#
# Windows note: upstream only shipped a linux script. this now picks the
# right sdk package for the host so vsd init can run under git bash.


set -e

ZEPHYR_SDK_VERSION=$1
ZEPHYR_SDK_INSTALL_DIR=$2

if [ "$1" == "" ] || [ "$2" == "" ]; then
  echo "expected 2 arguments" >&2
  echo "usage: ./get_zephyr_sdk.sh <zephyr-sdk-version> <zephyr-sdk-install-dir>" >&2
  exit 1
fi

BASE_URL="https://github.com/zephyrproject-rtos/sdk-ng/releases/download/v${ZEPHYR_SDK_VERSION}"

case "$(uname -s)" in
  MINGW*|MSYS*|CYGWIN*)
    PKG="zephyr-sdk-${ZEPHYR_SDK_VERSION}_windows-x86_64_minimal.zip"
    echo "downloading ${PKG}"
    curl -kLs "${BASE_URL}/${PKG}" -o "${ZEPHYR_SDK_INSTALL_DIR}/${PKG}"
    tar xf "${ZEPHYR_SDK_INSTALL_DIR}/${PKG}" -C "${ZEPHYR_SDK_INSTALL_DIR}" --strip-components=1
    rm -f "${ZEPHYR_SDK_INSTALL_DIR}/${PKG}"
    if [ -f "${ZEPHYR_SDK_INSTALL_DIR}/setup.cmd" ]; then
      (cd "${ZEPHYR_SDK_INSTALL_DIR}" && cmd //c setup.cmd -t all -h -c)
    fi
    ;;
  *)
    curl -kLs "${BASE_URL}/zephyr-sdk-${ZEPHYR_SDK_VERSION}_linux-x86_64_minimal.tar.xz" | tar xJ --strip 1 -C "${ZEPHYR_SDK_INSTALL_DIR}"
    cd "${ZEPHYR_SDK_INSTALL_DIR}"
    ./setup.sh -t all -h -c
    ;;
esac

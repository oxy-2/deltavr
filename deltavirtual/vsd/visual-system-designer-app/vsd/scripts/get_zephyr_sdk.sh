#!/usr/bin/env sh

# Copyright (c) 2022-2024 Antmicro <www.antmicro.com>
#
# SPDX-License-Identifier: Apache-2.0
#
# Windows note: upstream only shipped a linux script.
# - the windows sdk is a .7z, not .zip/.tar.xz
# - git-bash tar treats "C:\..." as a remote host
# - setup.cmd needs wget + 7z which windows boxes often lack
# so the windows path downloads and unpacks in python, then registers cmake.

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
    echo "installing zephyr sdk ${ZEPHYR_SDK_VERSION} into ${ZEPHYR_SDK_INSTALL_DIR}"
    python -c "
import os, pathlib, shutil, subprocess, sys, urllib.request

version, dest_s, base = sys.argv[1], sys.argv[2], sys.argv[3]
dest = pathlib.Path(dest_s)
dest.mkdir(parents=True, exist_ok=True)

def fetch(name):
    url = f'{base}/{name}'
    out = dest / name
    print('downloading', name, flush=True)
    req = urllib.request.Request(url, headers={'User-Agent': 'vsd-bootstrap'})
    with urllib.request.urlopen(req) as r, open(out, 'wb') as f:
        shutil.copyfileobj(r, f)
    print('saved', out, 'bytes', out.stat().st_size, flush=True)
    return out

def unpack_sevenz(archive):
    # windows tar (bsdtar) understands 7z and normal windows paths
    tar = r'C:\Windows\System32\tar.exe'
    print('extracting', archive.name, flush=True)
    subprocess.check_call([tar, 'xf', str(archive)], cwd=str(dest))
    archive.unlink()

# 1. sdk skeleton
skel = fetch(f'zephyr-sdk-{version}_windows-x86_64_minimal.7z')
unpack_sevenz(skel)
# flatten the single top-level folder
top = dest / f'zephyr-sdk-{version}'
if top.is_dir():
    for item in top.iterdir():
        target = dest / item.name
        if target.exists():
            if target.is_dir():
                shutil.rmtree(target)
            else:
                target.unlink()
        shutil.move(str(item), str(target))
    top.rmdir()

# 2. toolchains. arm covers the stm32 boards in deltavirtual.
#    add more names here if you need another arch.
toolchains = ['arm-zephyr-eabi']
for t in toolchains:
    if (dest / t).is_dir():
        print('toolchain already present:', t, flush=True)
        continue
    name = f'toolchain_windows-x86_64_{t}.7z'
    archive = fetch(name)
    unpack_sevenz(archive)

# 3. register cmake package so zephyr can find the sdk
cmake = shutil.which('cmake')
if cmake:
    print('registering cmake package', flush=True)
    subprocess.call([cmake, '-P', str(dest / 'cmake' / 'zephyr_sdk_export.cmake')], cwd=str(dest))

print('sdk ready at', dest, flush=True)
" "${ZEPHYR_SDK_VERSION}" "${ZEPHYR_SDK_INSTALL_DIR}" "${BASE_URL}"
    ;;
  *)
    curl -kLs "${BASE_URL}/zephyr-sdk-${ZEPHYR_SDK_VERSION}_linux-x86_64_minimal.tar.xz" | tar xJ --strip 1 -C "${ZEPHYR_SDK_INSTALL_DIR}"
    cd "${ZEPHYR_SDK_INSTALL_DIR}"
    ./setup.sh -t all -h -c
    ;;
esac

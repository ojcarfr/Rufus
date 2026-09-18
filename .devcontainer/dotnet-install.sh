#!/usr/bin/env bash

set -euo pipefail

install_dir="${DOTNET_INSTALL_DIR:-/usr/share/dotnet}"

case "$(uname -m)" in
    x86_64) architecture="x64" ;;
    aarch64) architecture="arm64" ;;
    *)
        echo "Unsupported architecture: $(uname -m)" >&2
        exit 1
        ;;
esac

mkdir -p "$install_dir"

archives=()
for version in "$@"; do
    archive="$(mktemp)"
    archives+=("$archive")
    curl --fail --location --show-error --silent \
        "https://dotnetcli.blob.core.windows.net/dotnet/Sdk/${version}/dotnet-sdk-${version}-linux-${architecture}.tar.gz" \
        --output "$archive" &
done
wait

for archive in "${archives[@]}"; do
    tar --extract --gzip --file "$archive" --directory "$install_dir"
    rm -f "$archive"
done

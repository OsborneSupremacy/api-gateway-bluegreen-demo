#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

FRAMEWORK="net10.0"
RUNTIME="linux-arm64"
CONFIGURATION="Release"

build_lambda() {
  local project_dir="$1"
  local zip_name="$2"

  echo "Building ${project_dir}/${zip_name}..."

  pushd "${project_dir}" >/dev/null

  dotnet publish \
    -o bin/publish \
    -c "${CONFIGURATION}" \
    --framework "${FRAMEWORK}" \
    /p:GenerateRuntimeConfigurationFiles=true \
    --runtime "${RUNTIME}" \
    --self-contained false

  pushd bin/publish >/dev/null
  rm -f "../${zip_name}"
  zip -rq "../${zip_name}" .
  popd >/dev/null

  popd >/dev/null
}

build_lambda "${REPO_ROOT}/src/Ecommerce/Ecommerce.Order.Create" "CreateOrder.zip"
build_lambda "${REPO_ROOT}/src/Ecommerce/Ecommerce.Order.Get" "GetOrder.zip"
build_lambda "${REPO_ROOT}/src/Ecommerce/Ecommerce.Order.Update" "UpdateOrder.zip"
build_lambda "${REPO_ROOT}/src/Ecommerce/Ecommerce.Order.Delete" "DeleteOrder.zip"
build_lambda "${REPO_ROOT}/src/Ecommerce/Ecommerce.Authorizer" "Authorizer.zip"

echo "Done. Lambda packages created:"
echo "- src/Ecommerce/Ecommerce.Order.Create/bin/CreateOrder.zip"
echo "- src/Ecommerce/Ecommerce.Order.Get/bin/GetOrder.zip"
echo "- src/Ecommerce/Ecommerce.Order.Update/bin/UpdateOrder.zip"
echo "- src/Ecommerce/Ecommerce.Order.Delete/bin/DeleteOrder.zip"
echo "- src/Ecommerce/Ecommerce.Authorizer/bin/Authorizer.zip"

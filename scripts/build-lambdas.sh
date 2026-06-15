#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

RUNTIME="linux-arm64"
CONFIGURATION="Release"

# Container image providing the Amazon Linux 2023 arm64 Native AOT toolchain (clang, zlib, etc.).
# Native AOT must compile against the target OS/architecture, so AOT functions are built inside
# this container rather than on the host.
AOT_BUILD_IMAGE="${AOT_BUILD_IMAGE:-public.ecr.aws/sam/build-dotnet10:latest-arm64}"

# Build a Native AOT Lambda that runs on the provided.al2023 custom runtime.
# Compilation happens inside the AL2023 arm64 container so the native toolchain links
# against the Lambda target. The output is a single `bootstrap` executable.
build_lambda_aot() {
  local project_dir="$1"
  local zip_name="$2"
  local project_file="$3"

  echo "Building (Native AOT) ${project_dir}/${zip_name}..."

  local rel_project_dir="${project_dir#"${REPO_ROOT}/"}"
  # The AOT-published executable is named after the assembly; the provided.al2023
  # custom runtime requires it to be named `bootstrap`.
  local assembly_name="${project_file%.csproj}"

  rm -f "${project_dir}/bin/${zip_name}"

  docker run --rm \
    --platform linux/arm64 \
    --entrypoint /bin/bash \
    -v "${REPO_ROOT}":/workspace \
    -w "/workspace/${rel_project_dir}" \
    "${AOT_BUILD_IMAGE}" \
    -c "dotnet publish ${project_file} -o bin/publish -c ${CONFIGURATION} --runtime ${RUNTIME} --self-contained true"

  pushd "${project_dir}/bin/publish" >/dev/null
  if [[ ! -f bootstrap ]]; then
    if [[ -f "${assembly_name}" ]]; then
      mv "${assembly_name}" bootstrap
    else
      echo "error: Native AOT publish did not produce an executable in ${project_dir}/bin/publish" >&2
      exit 1
    fi
  fi
  rm -f "../${zip_name}"
  zip -q "../${zip_name}" bootstrap
  popd >/dev/null
}

build_lambda_aot "${REPO_ROOT}/src/Ecommerce/Ecommerce.Order.Create" "CreateOrder.zip" "Ecommerce.Order.Create.csproj"
build_lambda_aot "${REPO_ROOT}/src/Ecommerce/Ecommerce.Order.Get" "GetOrder.zip" "Ecommerce.Order.Get.csproj"
build_lambda_aot "${REPO_ROOT}/src/Ecommerce/Ecommerce.Order.Update" "UpdateOrder.zip" "Ecommerce.Order.Update.csproj"
build_lambda_aot "${REPO_ROOT}/src/Ecommerce/Ecommerce.Order.Delete" "DeleteOrder.zip" "Ecommerce.Order.Delete.csproj"
build_lambda_aot "${REPO_ROOT}/src/Ecommerce/Ecommerce.Authorizer" "Authorizer.zip" "Ecommerce.Authorizer.csproj"

echo "Done. Lambda packages created:"
echo "- src/Ecommerce/Ecommerce.Order.Create/bin/CreateOrder.zip"
echo "- src/Ecommerce/Ecommerce.Order.Get/bin/GetOrder.zip"
echo "- src/Ecommerce/Ecommerce.Order.Update/bin/UpdateOrder.zip"
echo "- src/Ecommerce/Ecommerce.Order.Delete/bin/DeleteOrder.zip"
echo "- src/Ecommerce/Ecommerce.Authorizer/bin/Authorizer.zip"

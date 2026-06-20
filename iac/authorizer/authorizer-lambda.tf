module "authorizer_lambda" {
  source              = "../modules/lambda-function"
  function_name       = "${var.application_name}-authorizer"
  description         = "Lambda authorizer for the ecommerce API."
  lambda_package_path = "../../src/Ecommerce/Ecommerce.Authorizer/bin/Authorizer.zip"
  aws_region          = data.aws_region.current.region
  versioning_strategy = "" # authorizer cannot be used with stage variables, so the blue-green deployment approach is not compatible with the authorizer. It is therefore deployed by its own pipeline, independent of the blue-green infrastructure.
  environment_variables = {
    API_TOKEN = var.api_token == "" ? random_password.api_token.result : var.api_token
  }
}

resource "random_password" "api_token" {
  length  = 32
  special = false
}

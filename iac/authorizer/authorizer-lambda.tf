module "authorizer_lambda" {
  source              = "../modules/lambda-function"
  function_name       = "${var.application_name}-authorizer"
  description         = "Lambda authorizer for the ecommerce API."
  lambda_package_path = "../../src/Ecommerce/Ecommerce.Authorizer/bin/Authorizer.zip"
  aws_region          = data.aws_region.current.region
  # An API Gateway authorizer's URI can reference stage variables, so the authorizer uses the same
  # blue-green strategy as the order Lambdas: each apply publishes a new version that the green alias
  # points to, and the Deploy Authorizer pipeline promotes it to blue after testing.
  versioning_strategy = "blue_green"
  environment_variables = {
    API_TOKEN = var.api_token == "" ? random_password.api_token.result : var.api_token
  }
}

resource "random_password" "api_token" {
  length  = 32
  special = false
}

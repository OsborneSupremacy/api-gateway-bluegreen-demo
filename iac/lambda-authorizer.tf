module "authorizer_lambda" {
  source              = "./modules/lambda-function"
  function_name       = "${var.application_name}-authorizer"
  description         = "Lambda authorizer for the ecommerce API."
  lambda_handler      = "Ecommerce.Authorizer::Ecommerce.Authorizer.Function::FunctionHandler"
  lambda_package_path = "../src/Ecommerce/Ecommerce.Authorizer/bin/Authorizer.zip"
  aws_region          = data.aws_region.current.region
  versioning_strategy = "" # authorizer cannot be used with stage variables, so this blue-green deployment approach is not compatible with the authorizer.
  environment_variables = {
    API_TOKEN = var.api_token == "" ? random_password.api_token.result : var.api_token
  }
}

resource "aws_api_gateway_authorizer" "ecommerce_authorizer" {
  name                             = "${var.application_name}-authorizer"
  rest_api_id                      = aws_api_gateway_rest_api.ecommerce_gateway.id
  authorizer_uri                   = module.authorizer_lambda.latest_invoke_arn
  authorizer_result_ttl_in_seconds = 0
  identity_source                  = "method.request.header.Authorization"
  type                             = "TOKEN"
}

resource "aws_lambda_permission" "authorizer_api_gateway_invoke" {
  statement_id  = "AllowAPIGatewayInvokeAuthorizer"
  action        = "lambda:InvokeFunction"
  function_name = module.authorizer_lambda.latest_arn
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:us-east-1:${data.aws_caller_identity.current.account_id}:${aws_api_gateway_rest_api.ecommerce_gateway.id}/authorizers/${aws_api_gateway_authorizer.ecommerce_authorizer.id}"
}

resource "random_password" "api_token" {
  length  = 32
  special = false
}

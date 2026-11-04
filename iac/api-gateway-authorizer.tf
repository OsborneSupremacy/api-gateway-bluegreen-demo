# The authorizer Lambda function and its API token live in a separate Terraform
# project (iac/authorizer) deployed by an independent pipeline. This project
# consumes the function's outputs from the authorizer project's remote state.
# The authorizer project must be applied before this project (see README).
data "terraform_remote_state" "authorizer" {
  backend = var.authorizer_remote_state_backend
  config  = var.authorizer_remote_state_config
}

# Like the order Lambda integrations, the authorizer URI references the `alias`
# stage variable, so the blue stage invokes the authorizer's blue alias and the
# green stage invokes its green alias.
resource "aws_api_gateway_authorizer" "ecommerce_authorizer" {
  name                             = "${var.application_name}-authorizer"
  rest_api_id                      = aws_api_gateway_rest_api.ecommerce_gateway.id
  authorizer_uri                   = data.terraform_remote_state.authorizer.outputs.blue_green_stage_variable_invoke_arn
  authorizer_result_ttl_in_seconds = 0
  identity_source                  = "method.request.header.Authorization"
  type                             = "TOKEN"
}

locals {
  authorizer_source_arn = "arn:aws:execute-api:${data.aws_region.current.region}:${data.aws_caller_identity.current.account_id}:${aws_api_gateway_rest_api.ecommerce_gateway.id}/authorizers/${aws_api_gateway_authorizer.ecommerce_authorizer.id}"
}

# Keep the permission on the unqualified function so that there's no downtime when first opting-into blue-green deployment
# (until the blue stage is redeployed, it still invokes the unqualified authorizer). Once fully migrated, it could be removed,
# but it doesn't cause any issues to leave it in place.
resource "aws_lambda_permission" "authorizer_api_gateway_invoke" {
  statement_id  = "AllowAPIGatewayInvokeAuthorizer"
  action        = "lambda:InvokeFunction"
  function_name = data.terraform_remote_state.authorizer.outputs.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = local.authorizer_source_arn
}

resource "aws_lambda_permission" "authorizer_api_gateway_invoke_blue" {
  statement_id  = "AllowAPIGatewayInvokeAuthorizer"
  action        = "lambda:InvokeFunction"
  function_name = data.terraform_remote_state.authorizer.outputs.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = local.authorizer_source_arn
  qualifier     = var.blue_stage_name
}

resource "aws_lambda_permission" "authorizer_api_gateway_invoke_green" {
  statement_id  = "AllowAPIGatewayInvokeAuthorizer"
  action        = "lambda:InvokeFunction"
  function_name = data.terraform_remote_state.authorizer.outputs.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = local.authorizer_source_arn
  qualifier     = var.green_stage_name
}

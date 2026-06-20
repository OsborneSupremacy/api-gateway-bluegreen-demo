# The authorizer Lambda function and its API token live in a separate Terraform
# project (iac/authorizer) deployed by an independent pipeline. The authorizer
# cannot participate in the blue-green stage-variable scheme, so it is decoupled
# from this project. This project consumes the function's outputs from the
# authorizer project's remote state. The authorizer project must be applied
# before this project (see README).
data "terraform_remote_state" "authorizer" {
  backend = "s3"
  config = {
    bucket = "bro-tfstate"
    key    = "ecommerce-authorizer"
    region = "us-east-1"
  }
}

resource "aws_api_gateway_authorizer" "ecommerce_authorizer" {
  name                             = "${var.application_name}-authorizer"
  rest_api_id                      = aws_api_gateway_rest_api.ecommerce_gateway.id
  authorizer_uri                   = data.terraform_remote_state.authorizer.outputs.invoke_arn
  authorizer_result_ttl_in_seconds = 0
  identity_source                  = "method.request.header.Authorization"
  type                             = "TOKEN"
}

resource "aws_lambda_permission" "authorizer_api_gateway_invoke" {
  statement_id  = "AllowAPIGatewayInvokeAuthorizer"
  action        = "lambda:InvokeFunction"
  function_name = data.terraform_remote_state.authorizer.outputs.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${data.aws_region.current.region}:${data.aws_caller_identity.current.account_id}:${aws_api_gateway_rest_api.ecommerce_gateway.id}/authorizers/${aws_api_gateway_authorizer.ecommerce_authorizer.id}"
}

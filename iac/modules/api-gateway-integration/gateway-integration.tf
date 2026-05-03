resource "aws_api_gateway_integration" "lambda_integration" {
  rest_api_id             = var.gateway_rest_api_id
  resource_id             = var.gateway_resource_id
  http_method             = var.gateway_http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri =                    "arn:aws:apigateway:${data.aws_region.current.region}:lambda:path/2015-03-31/functions/${var.lambda_function_arn}:$${stageVariables.alias}/invocations"
  # uri                     = var.lambda_invoke_arn # standard convention, but can't use it here becaause it doesn't support stage variable substitution
  content_handling        = "CONVERT_TO_TEXT"
}

# Allow API Gateway to invoke any version, alias, or $LATEST of the Lambda function
resource "aws_lambda_permission" "api_gateway_invoke" {
  statement_id  = "AllowAPIGatewayInvoke-${var.gateway_http_method}-${var.gateway_rest_api_id}"
  action        = "lambda:InvokeFunction"
  function_name = var.lambda_function_arn
  principal     = "apigateway.amazonaws.com"
  # arn:aws:execute-api:{region}:{account}:{api-id}/*/{method}/{resource}
  # The :* qualifier grants permission for all versions, aliases, and $LATEST
  source_arn    = "arn:aws:execute-api:${data.aws_region.current.region}:${data.aws_caller_identity.current.account_id}:${var.gateway_rest_api_id}/*/*"
  qualifier     = null
}
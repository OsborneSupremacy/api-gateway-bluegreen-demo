resource "aws_api_gateway_integration" "lambda_integration" {
  rest_api_id             = var.gateway_rest_api_id
  resource_id             = var.gateway_resource_id
  http_method             = var.gateway_http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "arn:aws:apigateway:${var.aws_region}:lambda:path/2015-03-31/functions/${var.lambda_function_arn}:$${stageVariables.alias}/invocations"
  # uri                     = var.lambda_invoke_arn # standard convention, but can't use it here becaause it doesn't support stage variable substitution
  content_handling = "CONVERT_TO_TEXT"
}

# Keep the permission on the unqualified function so that there's no downtime when first opting-into blue-green deployment.
# Once fully migrated to blue-green, the unqualified permission could be removed if desired, but it doesn't cause any issues to leave it in place.
resource "aws_lambda_permission" "api_gateway_invoke" {
  statement_id  = "AllowAPIGatewayInvoke-${var.gateway_http_method}-${var.gateway_rest_api_id}"
  action        = "lambda:InvokeFunction"
  function_name = var.lambda_function_arn
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.aws_region}:${var.aws_account_id}:${var.gateway_rest_api_id}/*/*"
}

resource "aws_lambda_permission" "api_gateway_invoke_blue" {
  statement_id  = "AllowAPIGatewayInvoke-${var.gateway_http_method}-${var.gateway_rest_api_id}"
  action        = "lambda:InvokeFunction"
  function_name = var.lambda_function_arn
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.aws_region}:${var.aws_account_id}:${var.gateway_rest_api_id}/*/*"
  qualifier     = var.blue_stage_name
}

resource "aws_lambda_permission" "api_gateway_invoke_green" {
  statement_id  = "AllowAPIGatewayInvoke-${var.gateway_http_method}-${var.gateway_rest_api_id}"
  action        = "lambda:InvokeFunction"
  function_name = var.lambda_function_arn
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.aws_region}:${var.aws_account_id}:${var.gateway_rest_api_id}/*/*"
  qualifier     = var.green_stage_name
}

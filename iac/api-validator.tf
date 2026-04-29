# We could create validators for each integration, but API Gateway has a limit of 10 request validators per REST API..
# Also, there are only 3 combinations of request body and parameters validation,
# so we can create a validator for each combination at the API level and then allow each integration to specify which one to use.

resource "aws_api_gateway_request_validator" "request_body_validator" {
  name                        = "${aws_api_gateway_rest_api.ecommerce_gateway.name}-body-validator"
  rest_api_id                 = aws_api_gateway_rest_api.ecommerce_gateway.id
  validate_request_body       = true
  validate_request_parameters = false
}

resource "aws_api_gateway_request_validator" "request_parameters_validator" {
  name                        = "${aws_api_gateway_rest_api.ecommerce_gateway.name}-parameters-validator"
  rest_api_id                 = aws_api_gateway_rest_api.ecommerce_gateway.id
  validate_request_body       = false
  validate_request_parameters = true
}

resource "aws_api_gateway_request_validator" "request_body_and_parameters_validator" {
  name                        = "${aws_api_gateway_rest_api.ecommerce_gateway.name}-body-and-parameters-validator"
  rest_api_id                 = aws_api_gateway_rest_api.ecommerce_gateway.id
  validate_request_body       = true
  validate_request_parameters = true
}
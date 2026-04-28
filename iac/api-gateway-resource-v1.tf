resource "aws_api_gateway_resource" "v1" {
  rest_api_id = aws_api_gateway_rest_api.ecommerce_gateway.id
  parent_id   = aws_api_gateway_rest_api.ecommerce_gateway.root_resource_id
  path_part   = "v1"
}

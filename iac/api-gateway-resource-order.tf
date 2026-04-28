resource "aws_api_gateway_resource" "order" {
  rest_api_id = aws_api_gateway_rest_api.ecommerce_gateway.id
  parent_id   = aws_api_gateway_resource.v1.id
  path_part   = "order"
}

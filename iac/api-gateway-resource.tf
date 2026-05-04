resource "aws_api_gateway_resource" "v1" {
  rest_api_id = aws_api_gateway_rest_api.ecommerce_gateway.id
  parent_id   = aws_api_gateway_rest_api.ecommerce_gateway.root_resource_id
  path_part   = "v1"
}

resource "aws_api_gateway_resource" "order" {
  rest_api_id = aws_api_gateway_rest_api.ecommerce_gateway.id
  parent_id   = aws_api_gateway_resource.v1.id
  path_part   = "order"
}

resource "aws_api_gateway_resource" "order_customer_id" {
  rest_api_id = aws_api_gateway_rest_api.ecommerce_gateway.id
  parent_id   = aws_api_gateway_resource.order.id
  path_part   = "{customerId}"
}

resource "aws_api_gateway_resource" "order_customer_id_order_id" {
  rest_api_id = aws_api_gateway_rest_api.ecommerce_gateway.id
  parent_id   = aws_api_gateway_resource.order_customer_id.id
  path_part   = "{orderId}"
}

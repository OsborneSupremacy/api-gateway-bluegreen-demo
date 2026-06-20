resource "aws_api_gateway_rest_api" "ecommerce_gateway" {
  name                         = "${var.application_name}-gateway"
  disable_execute_api_endpoint = true
  description                  = "API Gateway for the ecommerce application"
  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

resource "aws_api_gateway_deployment" "ecommerce_deployment" {
  rest_api_id = aws_api_gateway_rest_api.ecommerce_gateway.id
  description = "Deployment for ecommerce API Gateway"
  lifecycle {
    create_before_destroy = true
  }
  depends_on = [
    aws_api_gateway_authorizer.ecommerce_authorizer,
    module.create_order_api_gateway_integration,
    module.get_order_api_gateway_integration,
    module.update_order_api_gateway_integration,
    module.delete_order_api_gateway_integration
  ]
}

resource "aws_api_gateway_stage" "blue_stage" {
  stage_name    = var.blue_stage_name
  description   = "${var.blue_stage_name} (production-serving) stage for the ecommerce API Gateway"
  rest_api_id   = aws_api_gateway_rest_api.ecommerce_gateway.id
  deployment_id = aws_api_gateway_deployment.ecommerce_deployment.id
  lifecycle {
    ignore_changes = [deployment_id]
  }
  variables = {
    "alias" = var.blue_stage_name
  }
}

resource "aws_api_gateway_stage" "green_stage" {
  stage_name    = var.green_stage_name
  description   = "${var.green_stage_name} (candidate/incoming) stage for the ecommerce API Gateway"
  rest_api_id   = aws_api_gateway_rest_api.ecommerce_gateway.id
  deployment_id = aws_api_gateway_deployment.ecommerce_deployment.id
  lifecycle {
    ignore_changes = [deployment_id]
  }
  variables = {
    "alias" = var.green_stage_name
  }
}

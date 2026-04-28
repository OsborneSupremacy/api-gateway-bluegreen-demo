resource "aws_api_gateway_rest_api" "ecommerce_gateway" {
  name        = "${var.application_name}-gateway"
  description = "API Gateway for the ecommerce application"
  endpoint_configuration {
    types = ["REGIONAL"]
  }
  # since this is a demo, we'll only allow access from the current IP address
  policy = jsonencode({
    "Version" : "2012-10-17",
    "Statement" : [
      {
        "Effect" : "Allow",
        "Principal" : {
          "AWS" : "*"
        }
        "Action" : "execute-api:Invoke",
        "Resource" : "arn:aws:execute-api:us-east-1:${data.aws_caller_identity.current.account_id}:*/*/*"
        "Condition" : {
          "IpAddress" : {
            "aws:SourceIp" : data.http.ipify.response_body
          }
        }
      }
    ]
  })
}

resource "aws_api_gateway_deployment" "ecommerce_deployment" {
  rest_api_id = aws_api_gateway_rest_api.ecommerce_gateway.id
  description = "Deployment for ecommerce API Gateway"
  lifecycle {
    create_before_destroy = true
  }
  depends_on = [
    module.create_order_lambda
  ]
}

resource "aws_api_gateway_stage" "blue_stage" {
  stage_name    = "blue"
  description = "Blue (production-serving) stage for the ecommerce API Gateway"
  rest_api_id   = aws_api_gateway_rest_api.ecommerce_gateway.id
  deployment_id = aws_api_gateway_deployment.ecommerce_deployment.id
}

resource "aws_api_gateway_stage" "green_stage" {
  stage_name    = "green"
  description = "Green (candidate/incoming) stage for the ecommerce API Gateway"
  rest_api_id   = aws_api_gateway_rest_api.ecommerce_gateway.id
  deployment_id = aws_api_gateway_deployment.ecommerce_deployment.id
}
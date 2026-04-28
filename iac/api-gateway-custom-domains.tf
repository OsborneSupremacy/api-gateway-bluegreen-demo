# Custom domain name for blue stage of the API Gateway
resource "aws_api_gateway_domain_name" "api_blue" {
  domain_name              = local.api_domain_name
  regional_certificate_arn = aws_acm_certificate_validation.api_domains.certificate_arn
  endpoint_configuration {
    types = ["REGIONAL"]
  }
  depends_on = [aws_route53_record.api_certificate_validation]
}

# base path mapping for blue stage
resource "aws_api_gateway_base_path_mapping" "api_blue" {
  api_id      = aws_api_gateway_rest_api.ecommerce_gateway.id
  domain_name = aws_api_gateway_domain_name.api_blue.domain_name
  stage_name  = aws_api_gateway_stage.blue_stage.stage_name
}

# Custom domain name for green stage of the API Gateway
resource "aws_api_gateway_domain_name" "api_green" {
  domain_name              = local.green_api_domain_name
  regional_certificate_arn = aws_acm_certificate_validation.api_domains.certificate_arn
  endpoint_configuration {
    types = ["REGIONAL"]
  }
  depends_on = [aws_route53_record.api_certificate_validation]
}

# base path mapping for green stage
resource "aws_api_gateway_base_path_mapping" "api_green" {
  api_id      = aws_api_gateway_rest_api.ecommerce_gateway.id
  domain_name = aws_api_gateway_domain_name.api_green.domain_name
  stage_name  = aws_api_gateway_stage.green_stage.stage_name
}

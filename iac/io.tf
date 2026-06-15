locals {
  ecommerce_domain_name = "${var.application_name}.${var.root_domain_name}"
  api_domain_name       = "api.${local.ecommerce_domain_name}"
  green_api_domain_name = "${var.green_stage_name}-api.${local.ecommerce_domain_name}"
}

# Inputs

variable "application_name" {
  description = "The name of the application. This will be used as a prefix for various resources, and part of the subdomain."
  type        = string
}

variable "orders_table_name" {
  description = "The name of the DynamoDB table for orders. The application name will be prepended to this value to create the full table name."
  type        = string
}

variable "root_domain_name" {
  description = "The existing Route 53 hosted zone domain name that owns the ecommerce subdomain. This should be a domain you control and have set up in Route53."
  type        = string
}

variable "api_token" {
  description = "Static bearer token accepted by the API authorizer. Omit to have a random token generated. This is only used for demonstration purposes - in a real application, you'd want a more robust authentication and authorization solution."
  type        = string
  sensitive   = true
  default     = ""
}

variable "blue_stage_name" {
  description = "The name of the API Gateway stage for the blue environment (production-serving)."
  type        = string
}

variable "green_stage_name" {
  description = "The name of the API Gateway stage for the green environment (incoming / candidate)."
  type        = string
}

variable "blue_cloudfront_web_acl_name" {
  description = "Optional Web ACL name to attach to the blue CloudFront distribution. Set to an empty string to create the distribution without a WAF association."
  type        = string
}

variable "green_cloudfront_web_acl_name" {
  description = "Optional Web ACL name to attach to the green CloudFront distribution. Set to an empty string to create the distribution without a WAF association."
  type        = string
}

variable "cloudfront_geo_whitelist" {
  description = "List of ISO 3166-1 alpha-2 country codes to whitelist for CloudFront geo restriction."
  type        = list(string)
  default     = []
}

variable "cloudfront_price_class" {
  description = "CloudFront price class for all distributions. Valid values: PriceClass_All, PriceClass_200, PriceClass_100."
  type        = string
  default     = "PriceClass_All"

  validation {
    condition     = contains(["PriceClass_All", "PriceClass_200", "PriceClass_100"], var.cloudfront_price_class)
    error_message = "cloudfront_price_class must be one of: PriceClass_All, PriceClass_200, PriceClass_100."
  }
}

output "ecommerce_domain_name" {
  description = "The delegated ecommerce subdomain used for API DNS records."
  value       = aws_route53_zone.ecommerce_subdomain.name
}

output "ecommerce_name_servers" {
  description = "The Route 53 name servers for the delegated ecommerce subdomain."
  value       = aws_route53_zone.ecommerce_subdomain.name_servers
}

output "api_domain_name" {
  description = "The blue environment API hostname."
  value       = local.api_domain_name
}

output "green_api_domain_name" {
  description = "The green environment API hostname."
  value       = local.green_api_domain_name
}

output "api_certificate_arn" {
  description = "The ACM certificate ARN for the API custom domains."
  value       = aws_acm_certificate_validation.api_domains.certificate_arn
}

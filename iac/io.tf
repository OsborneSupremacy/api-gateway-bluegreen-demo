locals {
  ecommerce_domain_name = "${var.application_name}.${var.root_domain_name}"
  api_domain_name       = "api.${local.ecommerce_domain_name}"
  green_api_domain_name = "green-api.${local.ecommerce_domain_name}"
}

# Inputs

variable "application_name" {
  description = "The name of the application. This will be used as a prefix for various resources, and part of the subdomain."
  type        = string
  default     = "ecommerce"
}

variable "orders_table_name" {
  description = "The name of the DynamoDB table for orders. The application name will be prepended to this value to create the full table name."
  type        = string
  default     = "orders"
}

variable "root_domain_name" {
  description = "The existing Route 53 hosted zone domain name that owns the ecommerce subdomain. This should be a domain you control and have set up in Route53."
  type        = string
  default     = "osbornesupremacy.com"
}

variable "api_token" {
  description = "Static bearer token accepted by the API authorizer."
  type        = string
  sensitive   = true
  default     = "conference-demo-token-change-me"
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

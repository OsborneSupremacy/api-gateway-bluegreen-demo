locals {
  ecommerce_domain_name = "${var.application_name}.${var.root_domain_name}"
  api_domain_name       = "api.${local.ecommerce_domain_name}"
  green_api_domain_name = "green-api.${local.ecommerce_domain_name}"
}

data "aws_route53_zone" "root_domain" {
  name         = var.root_domain_name
  private_zone = false
}

resource "aws_route53_zone" "ecommerce_subdomain" {
  name = local.ecommerce_domain_name
}

resource "aws_route53_record" "ecommerce_subdomain_ns" {
  zone_id = data.aws_route53_zone.root_domain.zone_id
  name    = aws_route53_zone.ecommerce_subdomain.name
  type    = "NS"
  ttl     = 300
  records = aws_route53_zone.ecommerce_subdomain.name_servers
}

resource "aws_acm_certificate" "api_domains" {
  domain_name               = local.api_domain_name
  subject_alternative_names = [local.green_api_domain_name]
  validation_method         = "DNS"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_route53_record" "api_certificate_validation" {
  for_each = {
    for option in aws_acm_certificate.api_domains.domain_validation_options : option.domain_name => {
      name   = option.resource_record_name
      record = option.resource_record_value
      type   = option.resource_record_type
    }
  }

  zone_id = aws_route53_zone.ecommerce_subdomain.zone_id
  name    = each.value.name
  type    = each.value.type
  ttl     = 60
  records = [each.value.record]
}

resource "aws_acm_certificate_validation" "api_domains" {
  certificate_arn         = aws_acm_certificate.api_domains.arn
  validation_record_fqdns = [for record in aws_route53_record.api_certificate_validation : record.fqdn]
}

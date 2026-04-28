
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


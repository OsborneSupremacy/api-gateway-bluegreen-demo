/*

Using custom subdomains for both blue and green API gateway stages.

AWS-generated URIs can be used, but custom domains provide a more user-friendly and branded experience for consumers accessing the API.

They also allow for easier management of SSL/TLS certificates and can improve SEO by providing a consistent domain name.

*/

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

resource "aws_route53_record" "api_blue_alias" {
  zone_id = aws_route53_zone.ecommerce_subdomain.zone_id
  name    = local.api_domain_name
  type    = "A"

  alias {
    name                   = aws_cloudfront_distribution.ecommerce_distribution.domain_name
    zone_id                = aws_cloudfront_distribution.ecommerce_distribution.hosted_zone_id
    evaluate_target_health = false
  }
}

resource "aws_route53_record" "api_green_alias" {
  zone_id = aws_route53_zone.ecommerce_subdomain.zone_id
  name    = local.green_api_domain_name
  type    = "A"

  alias {
    name                   = aws_cloudfront_distribution.ecommerce_distribution_green.domain_name
    zone_id                = aws_cloudfront_distribution.ecommerce_distribution_green.hosted_zone_id
    evaluate_target_health = false
  }
}


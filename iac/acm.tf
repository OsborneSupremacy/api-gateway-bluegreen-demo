resource "aws_acm_certificate" "api_domains" {
  domain_name               = local.api_domain_name
  subject_alternative_names = [local.green_api_domain_name]
  validation_method         = "DNS"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_acm_certificate_validation" "api_domains" {
  certificate_arn         = aws_acm_certificate.api_domains.arn
  validation_record_fqdns = [for record in aws_route53_record.api_certificate_validation : record.fqdn]
}

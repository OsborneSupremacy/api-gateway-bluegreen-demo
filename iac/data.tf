data "aws_route53_zone" "root_domain" {
  name         = var.root_domain_name
  private_zone = false
}

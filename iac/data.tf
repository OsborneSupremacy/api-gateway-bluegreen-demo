data "aws_caller_identity" "current" {}

data "aws_region" "current" {}

data "aws_route53_zone" "root_domain" {
  name         = var.root_domain_name
  private_zone = false
}

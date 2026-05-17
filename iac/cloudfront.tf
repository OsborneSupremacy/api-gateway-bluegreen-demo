data "aws_cloudfront_cache_policy" "caching_disabled" {
  name = "Managed-CachingDisabled"
}

data "aws_cloudfront_origin_request_policy" "all_viewer" {
  name = "Managed-AllViewer"
}

data "aws_wafv2_web_acl" "cloudfront_managed_blue" {
  count = var.blue_cloudfront_web_acl_name != "" ? 1 : 0
  name  = var.blue_cloudfront_web_acl_name
  scope = "CLOUDFRONT"
}

data "aws_wafv2_web_acl" "cloudfront_managed_green" {
  count = var.green_cloudfront_web_acl_name != "" ? 1 : 0
  name  = var.green_cloudfront_web_acl_name
  scope = "CLOUDFRONT"
}

locals {
  ecommerce_distribution_blue_origin_id  = "${aws_api_gateway_domain_name.api_blue.regional_domain_name}-origin"
  ecommerce_distribution_green_origin_id = "${aws_api_gateway_domain_name.api_green.regional_domain_name}-origin"
}

resource "aws_cloudfront_distribution" "ecommerce_distribution" {
  origin {
    domain_name = aws_api_gateway_domain_name.api_blue.regional_domain_name
    origin_id   = local.ecommerce_distribution_blue_origin_id

    custom_origin_config {
      http_port                = 80
      https_port               = 443
      origin_protocol_policy   = "https-only"
      origin_ssl_protocols     = ["TLSv1.2"]
      origin_keepalive_timeout = 5
      origin_read_timeout      = 30
    }
  }

  enabled         = true
  is_ipv6_enabled = true
  http_version    = "http2"
  price_class     = var.cloudfront_price_class
  web_acl_id      = var.blue_cloudfront_web_acl_name != "" ? data.aws_wafv2_web_acl.cloudfront_managed_blue[0].arn : null

  aliases = [
    local.api_domain_name
  ]

  default_cache_behavior {
    allowed_methods          = ["DELETE", "GET", "HEAD", "OPTIONS", "PATCH", "POST", "PUT"]
    cached_methods           = ["GET", "HEAD"]
    target_origin_id         = local.ecommerce_distribution_blue_origin_id
    viewer_protocol_policy   = "redirect-to-https"
    compress                 = true
    cache_policy_id          = data.aws_cloudfront_cache_policy.caching_disabled.id
    origin_request_policy_id = data.aws_cloudfront_origin_request_policy.all_viewer.id
  }

  restrictions {
    geo_restriction {
      restriction_type = length(var.cloudfront_geo_whitelist) > 0 ? "whitelist" : "none"
      locations        = var.cloudfront_geo_whitelist
    }
  }

  viewer_certificate {
    acm_certificate_arn      = aws_acm_certificate_validation.api_domains.certificate_arn
    ssl_support_method       = "sni-only"
    minimum_protocol_version = "TLSv1.2_2021"
  }
}

resource "aws_cloudfront_distribution" "ecommerce_distribution_green" {
  origin {
    domain_name = aws_api_gateway_domain_name.api_green.regional_domain_name
    origin_id   = local.ecommerce_distribution_green_origin_id

    custom_origin_config {
      http_port                = 80
      https_port               = 443
      origin_protocol_policy   = "https-only"
      origin_ssl_protocols     = ["TLSv1.2"]
      origin_keepalive_timeout = 5
      origin_read_timeout      = 30
    }
  }

  enabled         = true
  is_ipv6_enabled = true
  http_version    = "http2"
  price_class     = var.cloudfront_price_class
  web_acl_id      = var.green_cloudfront_web_acl_name != "" ? data.aws_wafv2_web_acl.cloudfront_managed_green[0].arn : null

  aliases = [
    local.green_api_domain_name
  ]

  default_cache_behavior {
    allowed_methods          = ["DELETE", "GET", "HEAD", "OPTIONS", "PATCH", "POST", "PUT"]
    cached_methods           = ["GET", "HEAD"]
    target_origin_id         = local.ecommerce_distribution_green_origin_id
    viewer_protocol_policy   = "redirect-to-https"
    compress                 = true
    cache_policy_id          = data.aws_cloudfront_cache_policy.caching_disabled.id
    origin_request_policy_id = data.aws_cloudfront_origin_request_policy.all_viewer.id
  }

  restrictions {
    geo_restriction {
      restriction_type = length(var.cloudfront_geo_whitelist) > 0 ? "whitelist" : "none"
      locations        = var.cloudfront_geo_whitelist
    }
  }

  viewer_certificate {
    acm_certificate_arn      = aws_acm_certificate_validation.api_domains.certificate_arn
    ssl_support_method       = "sni-only"
    minimum_protocol_version = "TLSv1.2_2021"
  }
}



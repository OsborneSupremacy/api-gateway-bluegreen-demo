application_name              = "ecommerce"
orders_table_name             = "orders"
root_domain_name              = "osbornesupremacy.com"
blue_cloudfront_web_acl_name  = "CreatedByCloudFront-37462c05"
green_cloudfront_web_acl_name = "CreatedByCloudFront-ff8ff432"
# Add country codes as needed, or leave the list empty to allow all countries (not recommended).
# Since I'm in the US and I'm the only consumer of this demo application, I'll just whitelist the US
# to reduce noise from any potential malicious actors in other countries scanning for open API endpoints.
cloudfront_geo_whitelist      = ["US"]
cloudfront_price_class        = "PriceClass_100"

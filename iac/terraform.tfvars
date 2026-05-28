application_name              = "ecommerce"
orders_table_name             = "orders"
blue_stage_name               = "blue"
green_stage_name              = "green"
root_domain_name              = "osbornesupremacy.com"
# These web ACLS were created manually in the AWS console. They are part of CloudFront's Free tier,
# which is a relatively new offering as of this writing. I'm not sure if they can be created in Terraform,
# so I'm not including them in the IaC for this demo application.
blue_cloudfront_web_acl_name  = "CreatedByCloudFront-37462c05"
green_cloudfront_web_acl_name = "CreatedByCloudFront-ff8ff432"
# Add country codes as needed, or leave the list empty to allow all countries (not recommended).
# Since I'm in the US and I'm the only consumer of this demo application, I'll just whitelist the US
# to reduce noise from any potential malicious actors in other countries scanning for open API endpoints.
cloudfront_geo_whitelist = ["US"]
cloudfront_price_class   = "PriceClass_All" # Required for Distributions with the Free pricing plan

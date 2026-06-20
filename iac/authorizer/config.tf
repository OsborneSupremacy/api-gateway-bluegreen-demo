provider "aws" {
  region = "us-east-1"

  default_tags {
    tags = {
      Application = "ecommerce"
      ManagedBy   = "terraform"
      Owner       = "ben@osbornesupremacy.com"
    }
  }
}

terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
    }
    random = {
      source = "hashicorp/random"
    }
  }

  backend "s3" {
    use_lockfile = true
  }
}

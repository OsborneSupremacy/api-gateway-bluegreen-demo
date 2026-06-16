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
  }

  backend "s3" {
    bucket       = "bro-tfstate"
    use_lockfile = true
    key          = "ecommerce"
    region       = "us-east-1"
  }
}

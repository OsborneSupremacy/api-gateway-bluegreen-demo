provider "aws" {
  region  = "us-east-1"
  profile = "benosborne"

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
    profile      = "benosborne"
    use_lockfile = true
    key          = "ecommerce"
    region       = "us-east-1"
  }
}

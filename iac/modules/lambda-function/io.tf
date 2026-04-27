
variable "function_name" {
  description = "The name of the Lambda function."
  type        = string
}

variable "description" {
  description = "A description of the Lambda function."
  type        = string
  default     = "A Lambda function for processing orders in the ecommerce application."
}

variable "lambda_handler" {
  description = "The handler for the Lambda function."
  type        = string
}

variable "lambda_package_path" {
  description = "The path to the Lambda function deployment package."
  type        = string
}

variable "environment_variables" {
  description = "A map of environment variables for the Lambda function."
  type        = map(string)
}

variable "orders_table_name" {
  description = "The name of the DynamoDB table for orders."
  type        = string
}

variable "orders_table_arn" {
  description = "The ARN of the DynamoDB table for orders."
  type        = string
}

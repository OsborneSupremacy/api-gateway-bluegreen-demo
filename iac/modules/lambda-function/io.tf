locals {
  use_blue_green  = var.versioning_strategy == "blue_green"
  publish_version = var.versioning_strategy != ""
}

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

variable "orders_table_arn" {
  description = "The ARN of the DynamoDB table for orders."
  type        = string
}

variable "versioning_strategy" {
  description = "The versioning strategy for the Lambda function. Use `blue_green` for a Blue/Green deployment strategy. Leave empty for no versioning. Any versioning strategy will enable versioning and aliases."
  type        = string
  default     = ""
  validation {
    condition     = var.versioning_strategy == "" || var.versioning_strategy == "blue_green"
    error_message = "versioning_strategy must be an empty string or 'blue_green'."
  }
}

# Outputs

output "function_name" {
  description = "The name of the Lambda function."
  value       = aws_lambda_function.lambda_function.function_name
}

output "latest_arn" {
  description = "The ARN of the Lambda function ($LATEST)."
  value       = aws_lambda_function.lambda_function.arn
}

output "latest_invoke_arn" {
  description = "The invoke ARN of the Lambda function ($LATEST)."
  value       = aws_lambda_function.lambda_function.invoke_arn
}

output "qualified_arn" {
  description = "The ARN of the Lambda function with version or alias qualifier. Available only when versioning is enabled."
  value       = local.publish_version ? aws_lambda_function.lambda_function.qualified_arn : null
}

output "qualified_invoke_arn" {
  description = "The invoke ARN of the Lambda function with version or alias qualifier. Available only when versioning is enabled."
  value       = local.publish_version ? aws_lambda_function.lambda_function.qualified_invoke_arn : null
}

output "version" {
  description = "The version of the Lambda function. Available only when versioning is enabled."
  value       = local.publish_version ? aws_lambda_function.lambda_function.version : null
}

output "blue_alias_arn" {
  description = "The ARN of the blue alias. Available only with blue_green versioning strategy."
  value       = local.use_blue_green ? aws_lambda_alias.blue_alias[0].arn : null
}

output "blue_alias_invoke_arn" {
  description = "The invoke ARN of the blue alias. Available only with blue_green versioning strategy."
  value       = local.use_blue_green ? aws_lambda_alias.blue_alias[0].invoke_arn : null
}

output "green_alias_arn" {
  description = "The ARN of the green alias. Available only with blue_green versioning strategy."
  value       = local.use_blue_green ? aws_lambda_alias.green_alias[0].arn : null
}

output "green_alias_invoke_arn" {
  description = "The invoke ARN of the green alias. Available only with blue_green versioning strategy."
  value       = local.use_blue_green ? aws_lambda_alias.green_alias[0].invoke_arn : null
}

output "previous_alias_arn" {
  description = "The ARN of the previous alias. Available only with blue_green versioning strategy."
  value       = local.use_blue_green ? aws_lambda_alias.previous_alias[0].arn : null
}

output "previous_alias_invoke_arn" {
  description = "The invoke ARN of the previous alias. Available only with blue_green versioning strategy."
  value       = local.use_blue_green ? aws_lambda_alias.previous_alias[0].invoke_arn : null
}

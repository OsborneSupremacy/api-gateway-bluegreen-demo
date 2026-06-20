# Inputs

variable "application_name" {
  description = "The name of the application. This will be used as a prefix for the authorizer function name."
  type        = string
}

variable "api_token" {
  description = "Static bearer token accepted by the API authorizer. Omit to have a random token generated. This is only used for demonstration purposes - in a real application, you'd want a more robust authentication and authorization solution."
  type        = string
  sensitive   = true
  default     = ""
}

# Outputs

output "function_name" {
  description = "The name of the authorizer Lambda function."
  value       = module.authorizer_lambda.function_name
}

output "function_arn" {
  description = "The ARN of the authorizer Lambda function ($LATEST)."
  value       = module.authorizer_lambda.latest_arn
}

output "invoke_arn" {
  description = "The invoke ARN of the authorizer Lambda function ($LATEST). Consumed by the main project's aws_api_gateway_authorizer."
  value       = module.authorizer_lambda.latest_invoke_arn
}

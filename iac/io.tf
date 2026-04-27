
variable "application_name" {
  description = "The name of the application."
  type        = string
  default     = "ecommerce"
}

variable "orders_table_name" {
  description = "The name of the DynamoDB table for orders."
  type        = string
  default     = "${var.application_name}-orders"
}

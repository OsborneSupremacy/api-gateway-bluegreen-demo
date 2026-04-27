
variable "application_name" {
  description = "The name of the application."
  type        = string
  default     = "ecommerce"
}

variable "orders_table_name" {
  description = "The name of the DynamoDB table for orders. The application name will be prepended to this value to create the full table name."
  type        = string
  default     = "orders"
}

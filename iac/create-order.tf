module "create_order_lambda" {
  source = "./modules/lambda-function"

  function_name       = "${var.application_name}-create-order"
  description         = "Lambda function for creating orders in the ecommerce application."
  lambda_handler      = "CreateOrder::CreateOrder.Function::FunctionHandler"
  lambda_package_path = "../src/Ecommerce/CreateOrder/bin/CreateOrder.zip"

  orders_table_name = aws_dynamodb_table.orders_table.name
  orders_table_arn  = aws_dynamodb_table.orders_table.arn

  environment_variables = {
    ORDERS_TABLE_NAME = aws_dynamodb_table.orders_table.name
  }
}

module "delete_order_lambda" {
  source = "./modules/lambda-function"

  function_name       = "${var.application_name}-delete-order"
  description         = "Lambda function for deleting an order in the ecommerce application."
  lambda_handler      = "Ecommerce.Order.Delete::Ecommerce.Order.Delete.Function::FunctionHandler"
  lambda_package_path = "../src/Ecommerce/Ecommerce.Order.Delete/bin/DeleteOrder.zip"
  versioning_strategy = "blue_green"

  orders_table_name = aws_dynamodb_table.orders_table.name
  orders_table_arn  = aws_dynamodb_table.orders_table.arn

  environment_variables = {
    ORDERS_TABLE_NAME = aws_dynamodb_table.orders_table.name
  }
}

module "delete_order_api_gateway_integration" {
  source = "./modules/api-gateway-integration"

  gateway_rest_api_id                               = aws_api_gateway_rest_api.ecommerce_gateway.id
  gateway_resource_id                               = aws_api_gateway_resource.order.id
  gateway_http_method                               = "DELETE"
  gateway_http_operation_name                       = "DeleteOrder"
  gateway_method_request_parameters                 = {}
  gateway_method_request_model_schema_file_location = "../schemas/delete-order-request.json"
  gateway_method_request_model_name                 = "DeleteOrderRequestModel"
  gateway_method_request_model_description          = "Model schema for the Delete Order API request body"
  lambda_invoke_arn                                 = module.delete_order_lambda.latest_invoke_arn
  lambda_function_arn                               = module.delete_order_lambda.latest_arn
  include_204_response                              = true
  include_404_response                              = false
  include_409_response                              = false
  good_response_model_name                          = ""
  good_response_model_description                   = ""
  good_response_model_schema_file_location          = ""
  authorizer_id                                     = aws_api_gateway_authorizer.ecommerce_authorizer.id
  request_validator_id                              = aws_api_gateway_request_validator.request_body_validator.id
}

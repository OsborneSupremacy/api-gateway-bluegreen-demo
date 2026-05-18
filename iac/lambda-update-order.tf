module "update_order_lambda" {
  source              = "./modules/lambda-function"
  function_name       = "${var.application_name}-update-order"
  description         = "Lambda function for updating an order in the ecommerce application."
  lambda_handler      = "Ecommerce.Order.Update::Ecommerce.Order.Update.Function::FunctionHandler"
  lambda_package_path = "../src/Ecommerce/Ecommerce.Order.Update/bin/UpdateOrder.zip"
  versioning_strategy = "blue_green"
  orders_table_arn    = aws_dynamodb_table.orders_table.arn
  environment_variables = {
    ORDERS_TABLE_NAME = aws_dynamodb_table.orders_table.name
  }
}

module "update_order_api_gateway_integration" {
  source                                            = "./modules/api-gateway-integration"
  blue_stage_name                                   = var.blue_stage_name
  green_stage_name                                  = var.green_stage_name
  gateway_rest_api_id                               = aws_api_gateway_rest_api.ecommerce_gateway.id
  gateway_resource_id                               = aws_api_gateway_resource.order.id
  gateway_http_method                               = "PUT"
  gateway_http_operation_name                       = "UpdateOrder"
  gateway_method_request_parameters                 = {}
  gateway_method_request_model_schema_file_location = "../schemas/update-order-request.json"
  gateway_method_request_model_name                 = "UpdateOrderRequestModel"
  gateway_method_request_model_description          = "Model schema for the Update Order API request body"
  lambda_invoke_arn                                 = module.update_order_lambda.latest_invoke_arn
  lambda_function_arn                               = module.update_order_lambda.latest_arn
  include_404_response                              = true
  include_409_response                              = false
  good_response_model_name                          = ""
  good_response_model_description                   = ""
  good_response_model_schema_file_location          = ""
  authorizer_id                                     = aws_api_gateway_authorizer.ecommerce_authorizer.id
  request_validator_id                              = aws_api_gateway_request_validator.request_body_validator.id
  aws_account_id                                    = data.aws_caller_identity.current.account_id
  aws_region                                        = data.aws_region.current.region
}

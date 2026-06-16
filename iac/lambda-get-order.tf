module "get_order_lambda" {
  source              = "./modules/lambda-function"
  function_name       = "${var.application_name}-get-order"
  description         = "Lambda function for getting an order in the ecommerce application."
  lambda_package_path = "../src/Ecommerce/Ecommerce.Order.Get/bin/GetOrder.zip"
  aws_region          = data.aws_region.current.region
  versioning_strategy = "blue_green"
  orders_table_arn    = aws_dynamodb_table.orders_table.arn
  environment_variables = {
    ORDERS_TABLE_NAME = aws_dynamodb_table.orders_table.name
  }
}

module "get_order_api_gateway_integration" {
  source                      = "./modules/api-gateway-integration"
  blue_stage_name             = var.blue_stage_name
  green_stage_name            = var.green_stage_name
  gateway_rest_api_id         = aws_api_gateway_rest_api.ecommerce_gateway.id
  gateway_resource_id         = aws_api_gateway_resource.order_customer_id_order_id.id
  gateway_http_method         = "GET"
  gateway_http_operation_name = "GetOrder"
  gateway_method_request_parameters = {
    "method.request.path.customerId" = "true"
    "method.request.path.orderId"    = "true"
  }
  gateway_method_request_model_schema_file_location = ""
  gateway_method_request_model_name                 = ""
  gateway_method_request_model_description          = ""
  lambda_invoke_arn                                 = module.get_order_lambda.latest_invoke_arn
  lambda_function_arn                               = module.get_order_lambda.latest_arn
  blue_green_stage_variable_invoke_arn              = module.get_order_lambda.blue_green_stage_variable_invoke_arn
  include_404_response                              = true
  include_409_response                              = false
  good_response_model_name                          = "GetOrderResponseModel"
  good_response_model_description                   = "Model schema for the Get Order API response body"
  good_response_model_schema_file_location          = "../schemas/get-order-response.json"
  authorizer_id                                     = aws_api_gateway_authorizer.ecommerce_authorizer.id
  request_validator_id                              = aws_api_gateway_request_validator.request_parameters_validator.id
  aws_account_id                                    = data.aws_caller_identity.current.account_id
  aws_region                                        = data.aws_region.current.region
}

resource "aws_lambda_function" "lambda_function" {
  function_name    = var.function_name
  description      = var.description
  handler          = var.native_aot ? "bootstrap" : var.lambda_handler
  runtime          = var.native_aot ? "provided.al2023" : "dotnet10"
  architectures    = ["arm64"]
  memory_size      = 128
  timeout          = 30
  filename         = var.lambda_package_path
  source_code_hash = filebase64sha256(var.lambda_package_path)
  role             = aws_iam_role.lambda_execution_role.arn
  publish          = local.publish_version
  environment {
    variables = var.environment_variables
  }
}

resource "aws_iam_role" "lambda_execution_role" {
  name = "${var.function_name}-execution-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_basic_execution" {
  role       = aws_iam_role.lambda_execution_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_iam_role_policy" "dynamo_db_access" {
  count = var.orders_table_arn != "" ? 1 : 0
  name  = "${var.function_name}-dynamodb-access"
  role  = aws_iam_role.lambda_execution_role.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "dynamodb:PutItem",
          "dynamodb:GetItem",
          "dynamodb:UpdateItem",
          "dynamodb:DeleteItem",
          "dynamodb:Scan",
          "dynamodb:Query"
        ]
        Resource = [
          var.orders_table_arn,
          "${var.orders_table_arn}/index/*" # Allow access to all indexes of the DynamoDB table
        ]
      }
    ]
  })
}

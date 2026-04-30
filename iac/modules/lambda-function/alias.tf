resource "aws_lambda_alias" "blue_alias" {
  count            = local.use_blue_green ? 1 : 0
  name             = "blue"
  description      = "Blue alias (production-serving) for ${var.function_name}"
  function_name    = aws_lambda_function.lambda_function.arn
  function_version = aws_lambda_function.lambda_function.version
  lifecycle {
    ignore_changes = [function_version]
    # this prevents Terraform from to updating the blue alias to point to the new version during deployment,
    # allowing for manual control over when the blue alias is switched to the new version
  }
}

resource "aws_lambda_alias" "green_alias" {
  count            = local.use_blue_green ? 1 : 0
  name             = "green"
  description      = "Green alias (candidate/incoming) for ${var.function_name}"
  function_name    = aws_lambda_function.lambda_function.arn
  function_version = aws_lambda_function.lambda_function.version
}

resource "aws_lambda_alias" "previous_alias" {
  count            = local.use_blue_green ? 1 : 0
  name             = "previous"
  description      = "Previous alias (previous production-serving) for ${var.function_name}"
  function_name    = aws_lambda_function.lambda_function.arn
  function_version = aws_lambda_function.lambda_function.version
  lifecycle {
    ignore_changes = [function_version]
  }
}

resource "aws_cloudwatch_log_group" "worker" {
  count = local.deploy_worker ? 1 : 0

  name              = "/aws/lambda/${local.name_prefix}-worker"
  retention_in_days = 14
}

resource "aws_lambda_function" "worker" {
  count = local.deploy_worker ? 1 : 0

  function_name = "${local.name_prefix}-worker"
  role          = aws_iam_role.worker_lambda.arn
  runtime       = var.worker_lambda_runtime
  handler       = var.worker_lambda_handler
  architectures = ["x86_64"]

  s3_bucket        = aws_s3_bucket.lambda_artifacts.id
  s3_key           = aws_s3_object.worker_lambda_package[0].key
  source_code_hash = filebase64sha256(var.worker_lambda_deployment_package_path)

  timeout     = var.worker_lambda_timeout_seconds
  memory_size = var.worker_lambda_memory_mb

  environment {
    variables = local.worker_lambda_environment
  }

  depends_on = [
    aws_cloudwatch_log_group.worker,
    aws_iam_role_policy_attachment.worker_lambda_basic_execution,
    aws_s3_object.worker_lambda_package,
  ]
}

resource "aws_lambda_event_source_mapping" "worker_sqs" {
  count = local.deploy_worker ? 1 : 0

  event_source_arn = aws_sqs_queue.team_processing.arn
  function_name    = aws_lambda_function.worker[0].arn
  batch_size       = 1
  enabled          = true
}

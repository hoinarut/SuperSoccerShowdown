output "ui_bucket_name" {
  description = "S3 bucket storing the Angular UI static assets."
  value       = aws_s3_bucket.ui.id
}

output "ui_cloudfront_url" {
  description = "Public HTTPS URL for the Angular UI."
  value       = "https://${aws_cloudfront_distribution.ui.domain_name}"
}

output "ui_cloudfront_distribution_id" {
  description = "CloudFront distribution ID for cache invalidation."
  value       = aws_cloudfront_distribution.ui.id
}

output "lambda_artifacts_bucket" {
  description = "S3 bucket storing Lambda deployment packages."
  value       = aws_s3_bucket.lambda_artifacts.id
}

output "lambda_package_s3_key" {
  description = "S3 object key for the deployed Lambda package. Empty until a package path is provided."
  value       = try(aws_s3_object.lambda_package[0].key, null)
}

output "lambda_function_name" {
  description = "Deployed Lambda function name. Empty until a deployment package is provided."
  value       = try(aws_lambda_function.api[0].function_name, null)
}

output "worker_lambda_function_name" {
  description = "Deployed worker Lambda function name. Empty until a worker deployment package is provided."
  value       = try(aws_lambda_function.worker[0].function_name, null)
}

output "team_processing_queue_url" {
  description = "SQS queue URL for asynchronous team processing messages."
  value       = aws_sqs_queue.team_processing.url
}

output "team_processing_queue_arn" {
  description = "SQS queue ARN for asynchronous team processing messages."
  value       = aws_sqs_queue.team_processing.arn
}

output "api_gateway_url" {
  description = "Public URL for the HTTP API. Empty until a deployment package is provided."
  value       = try(aws_apigatewayv2_stage.default[0].invoke_url, null)
}

output "database_secret_arn" {
  description = "Secrets Manager ARN containing the SQL Server connection string."
  value       = aws_secretsmanager_secret.database.arn
}

output "lambda_execution_role_arn" {
  description = "IAM role ARN used by the Lambda function."
  value       = aws_iam_role.lambda.arn
}

output "rds_endpoint" {
  description = "RDS endpoint when enable_rds is true."
  value       = try(aws_db_instance.main[0].address, null)
}

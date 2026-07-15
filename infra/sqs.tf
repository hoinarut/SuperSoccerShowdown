resource "aws_sqs_queue" "team_processing" {
  name                       = "${local.name_prefix}-team-processing"
  visibility_timeout_seconds = var.worker_lambda_timeout_seconds * 2
  message_retention_seconds  = var.sqs_message_retention_seconds
}

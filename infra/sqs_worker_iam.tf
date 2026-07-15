data "aws_iam_policy_document" "worker_lambda_assume_role" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "worker_lambda" {
  name               = "${local.name_prefix}-worker-lambda"
  assume_role_policy = data.aws_iam_policy_document.worker_lambda_assume_role.json
}

resource "aws_iam_role_policy_attachment" "worker_lambda_basic_execution" {
  role       = aws_iam_role.worker_lambda.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

data "aws_iam_policy_document" "worker_lambda_sqs" {
  statement {
    actions = [
      "sqs:ReceiveMessage",
      "sqs:DeleteMessage",
      "sqs:GetQueueAttributes",
      "sqs:SendMessage",
    ]

    resources = [
      aws_sqs_queue.team_processing.arn,
    ]
  }
}

resource "aws_iam_role_policy" "worker_lambda_sqs" {
  name   = "${local.name_prefix}-worker-lambda-sqs"
  role   = aws_iam_role.worker_lambda.id
  policy = data.aws_iam_policy_document.worker_lambda_sqs.json
}

locals {
  name_prefix = "${var.project_name}-${var.environment}"

  deploy_api    = var.lambda_deployment_package_path != ""
  deploy_worker = var.worker_lambda_deployment_package_path != ""

  connection_string = var.enable_rds ? (
    "Server=${aws_db_instance.main[0].address},${aws_db_instance.main[0].port};Database=${var.project_name};User ID=${var.db_username};Password=${var.db_password};TrustServerCertificate=true;Encrypt=true;"
  ) : var.database_connection_string

  cors_allowed_origins = [
    "http://localhost:4200",
    "http://127.0.0.1:4200",
    "https://${aws_cloudfront_distribution.ui.domain_name}",
  ]

  lambda_environment = merge(
    {
      ASPNETCORE_ENVIRONMENT               = var.aspnetcore_environment
      ConnectionStrings__DefaultConnection = local.connection_string
    },
    {
      for index, origin in local.cors_allowed_origins :
      "Cors__AllowedOrigins__${index}" => origin
    }
  )

  worker_lambda_environment = {
    API_BASE_URL = try(aws_apigatewayv2_stage.default[0].invoke_url, "")
    QUEUE_URL    = aws_sqs_queue.team_processing.url
    MAX_RETRIES  = tostring(var.sqs_max_retries)
  }
}

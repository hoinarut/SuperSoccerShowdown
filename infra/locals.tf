locals {
  name_prefix = "${var.project_name}-${var.environment}"

  deploy_api = var.lambda_deployment_package_path != ""

  cors_allowed_origins = [
    "http://localhost:4200",
    "http://127.0.0.1:4200",
    "https://${aws_cloudfront_distribution.ui.domain_name}",
  ]

  lambda_environment = merge(
    {
      ASPNETCORE_ENVIRONMENT = var.aspnetcore_environment
    },
    {
      for index, origin in local.cors_allowed_origins :
      "Cors__AllowedOrigins__${index}" => origin
    }
  )
}

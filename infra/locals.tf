locals {
  name_prefix = "${var.project_name}-${var.environment}"

  deploy_api = var.lambda_deployment_package_path != ""

  connection_string = var.enable_rds ? (
    "Server=${aws_db_instance.main[0].address},${aws_db_instance.main[0].port};Database=${var.project_name};User ID=${var.db_username};Password=${var.db_password};TrustServerCertificate=true;Encrypt=true;"
  ) : var.database_connection_string

  lambda_environment = {
    ASPNETCORE_ENVIRONMENT               = var.aspnetcore_environment
    ConnectionStrings__DefaultConnection = local.connection_string
  }
}

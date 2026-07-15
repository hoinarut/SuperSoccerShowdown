resource "aws_secretsmanager_secret" "database" {
  name        = "${local.name_prefix}/database-connection"
  description = "SQL Server connection string for the ${var.project_name} API."
}

resource "aws_secretsmanager_secret_version" "database" {
  secret_id = aws_secretsmanager_secret.database.id

  secret_string = jsonencode({
    ConnectionStrings__DefaultConnection = local.connection_string
  })
}

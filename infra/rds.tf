resource "aws_db_subnet_group" "main" {
  count = var.enable_rds ? 1 : 0

  name       = "${local.name_prefix}-db"
  subnet_ids = var.rds_subnet_ids
}

# Smallest public SQL Server Express instance (db.t3.micro, 20 GB gp3).
# Note: RDS SQL Server does not support creating a named database at provision time;
# the API connection string targets Database=sss-db (created by app migrations).
resource "aws_db_instance" "main" {
  count = var.enable_rds ? 1 : 0

  identifier = "${local.name_prefix}-sqlserver"

  engine         = "sqlserver-ex"
  engine_version = "15.00.4312.2.v1"
  license_model  = "license-included"
  instance_class = "db.t3.micro"

  allocated_storage = var.db_allocated_storage_gb
  storage_type      = "gp3"

  username = var.db_username
  password = local.db_password

  db_subnet_group_name   = aws_db_subnet_group.main[0].name
  vpc_security_group_ids = [aws_security_group.rds[0].id]

  backup_retention_period = 7
  skip_final_snapshot     = true
  deletion_protection     = false
  publicly_accessible     = true
  multi_az                = false

  apply_immediately = true
}

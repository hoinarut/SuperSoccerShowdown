variable "project_name" {
  description = "Short project name used as a prefix for AWS resources."
  type        = string
  default     = "sss"
}

variable "environment" {
  description = "Deployment environment label (for example: dev, staging, prod)."
  type        = string
  default     = "dev"
}

variable "aws_region" {
  description = "AWS region for all resources."
  type        = string
  default     = "eu-west-1"
}

variable "aspnetcore_environment" {
  description = "ASPNETCORE_ENVIRONMENT value passed to the Lambda function."
  type        = string
  default     = "Production"
}

variable "database_connection_string" {
  description = "SQL Server connection string for the API. Required unless enable_rds is true."
  type        = string
  sensitive   = true
  default     = ""
}

variable "lambda_deployment_package_path" {
  description = "Local path to the Lambda deployment zip published by CI. Leave empty to provision supporting resources without deploying the function."
  type        = string
  default     = ""
}

variable "lambda_package_version" {
  description = "Version label for the deployment artifact stored in S3 (for example a git commit SHA)."
  type        = string
  default     = "latest"
}

variable "lambda_runtime" {
  description = "AWS Lambda managed runtime for the API."
  type        = string
  default     = "dotnet10"
}

variable "lambda_handler" {
  description = "Lambda handler for ASP.NET Core hosting (assembly name)."
  type        = string
  default     = "MyApp.Api"
}

variable "lambda_memory_mb" {
  description = "Memory allocated to the Lambda function in MB."
  type        = number
  default     = 1024
}

variable "lambda_timeout_seconds" {
  description = "Lambda timeout in seconds."
  type        = number
  default     = 30
}

variable "api_gateway_stage_name" {
  description = "API Gateway stage name exposed in the public URL."
  type        = string
  default     = "$default"
}

variable "enable_rds" {
  description = "When true, provisions a VPC, NAT gateway, and SQL Server Express RDS instance."
  type        = bool
  default     = false
}

variable "db_username" {
  description = "Master username for the optional RDS SQL Server instance."
  type        = string
  default     = "sssadmin"
}

variable "db_password" {
  description = "Master password for the optional RDS SQL Server instance."
  type        = string
  sensitive   = true
  default     = ""
}

variable "db_allocated_storage_gb" {
  description = "Allocated storage for the optional RDS instance in GB."
  type        = number
  default     = 20
}

variable "vpc_cidr" {
  description = "CIDR block for the optional VPC."
  type        = string
  default     = "10.0.0.0/16"
}

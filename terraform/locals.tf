# Local values for the Todo List Application Infrastructure

locals {
  # Common naming convention
  name_prefix = "${var.project_name}-${var.environment}"

  # Common tags that should be applied to all resources
  common_tags = merge(var.tags, {
    ManagedBy    = "Terraform"
    Project      = var.project_name
    Environment  = var.environment
    DeployedDate = timestamp()
  })

  # Network configuration helpers
  vnet_name = "vnet-${local.name_prefix}"

  # Security groups
  allowed_locations = [
    var.location
  ]

  # Application configuration
  app_settings_common = {
    "ASPNETCORE_ENVIRONMENT"       = var.environment == "production" ? "Production" : title(var.environment)
    "WEBSITE_NODE_DEFAULT_VERSION" = "~18"
    "WEBSITE_RUN_FROM_PACKAGE"     = "1"
  }
}
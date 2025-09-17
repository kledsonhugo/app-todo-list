# Development Environment Configuration

environment         = "dev"
project_name        = "todolist"
location            = "East US"
resource_group_name = "rg-todolist-dev"

# Application Configuration
app_name             = "todolist-api"
app_service_plan_sku = "F1" # Free tier for development
dotnet_version       = "8.0"

# Networking Configuration
vnet_address_space = ["10.1.0.0/16"]
subnet_configs = {
  app_subnet = {
    address_prefixes  = ["10.1.1.0/24"]
    service_endpoints = ["Microsoft.Web", "Microsoft.Storage"]
  }
  private_endpoint_subnet = {
    address_prefixes  = ["10.1.2.0/24"]
    service_endpoints = ["Microsoft.Storage"]
  }
}

# Security Configuration (more permissive for dev)
allowed_ips = [
  "0.0.0.0/0" # Allow all for development
]

# Storage Configuration
storage_account_tier = "Standard"
storage_replication  = "LRS"

# Private Endpoints (disabled for cost savings in dev)
enable_private_endpoints = false
private_dns_zones        = []

# Tags
tags = {
  Environment = "development"
  Project     = "TodoList"
  Owner       = "Development Team"
  CostCenter  = "IT-Development"
  DeployedBy  = "Terraform"
}

# HTTPS Configuration (more lenient for dev)
enable_https_only = false
min_tls_version   = "1.0"

# Application Insights
enable_app_insights = true
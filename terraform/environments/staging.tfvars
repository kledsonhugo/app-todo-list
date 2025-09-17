# Staging Environment Configuration

environment         = "staging"
project_name        = "todolist"
location            = "East US"
resource_group_name = "rg-todolist-staging"

# Application Configuration
app_name             = "todolist-api"
app_service_plan_sku = "B1" # Basic tier for staging
dotnet_version       = "8.0"

# Networking Configuration
vnet_address_space = ["10.2.0.0/16"]
subnet_configs = {
  app_subnet = {
    address_prefixes  = ["10.2.1.0/24"]
    service_endpoints = ["Microsoft.Web", "Microsoft.Storage"]
  }
  private_endpoint_subnet = {
    address_prefixes  = ["10.2.2.0/24"]
    service_endpoints = ["Microsoft.Storage"]
  }
}

# Security Configuration
allowed_ips = [
  "203.0.113.0/24" # Example corporate network
]

# Storage Configuration
storage_account_tier = "Standard"
storage_replication  = "GRS" # Geo-redundant for staging

# Private Endpoints (enabled for staging to test production config)
enable_private_endpoints = true
private_dns_zones = [
  "privatelink.azurewebsites.net",
  "privatelink.blob.core.windows.net"
]

# Tags
tags = {
  Environment = "staging"
  Project     = "TodoList"
  Owner       = "QA Team"
  CostCenter  = "IT-Development"
  DeployedBy  = "Terraform"
}

# HTTPS Configuration
enable_https_only = true
min_tls_version   = "1.2"

# Application Insights
enable_app_insights = true
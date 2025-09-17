# External Variables for Todo List Application Infrastructure
# This file contains all configurable parameters for the infrastructure deployment

# Basic Configuration
environment         = "production"
project_name        = "todolist"
location            = "East US"
resource_group_name = "rg-todolist-prod"

# Application Configuration
app_name             = "todolist-api"
app_service_plan_sku = "B1" # Basic tier for demo
dotnet_version       = "8.0"

# Networking Configuration
vnet_address_space = ["10.0.0.0/16"]
subnet_configs = {
  app_subnet = {
    address_prefixes  = ["10.0.1.0/24"]
    service_endpoints = ["Microsoft.Web", "Microsoft.Storage"]
  }
  private_endpoint_subnet = {
    address_prefixes  = ["10.0.2.0/24"]
    service_endpoints = ["Microsoft.Storage"]
  }
}

# Security Configuration
allowed_ips = [
  "0.0.0.0/0" # Allow all for demo - restrict in production
]

# Storage Configuration
storage_account_tier = "Standard"
storage_replication  = "LRS" # Locally Redundant Storage for demo

# Private Endpoint Configuration
enable_private_endpoints = true
private_dns_zones = [
  "privatelink.azurewebsites.net",
  "privatelink.blob.core.windows.net"
]

# Tags
tags = {
  Environment = "production"
  Project     = "TodoList"
  Owner       = "DevOps Team"
  CostCenter  = "IT-Development"
  DeployedBy  = "Terraform"
}

# HTTPS Configuration
enable_https_only = true
min_tls_version   = "1.2"

# Application Insights
enable_app_insights = true
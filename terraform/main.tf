# Main Terraform Configuration for Todo List Application
# This file orchestrates all modules to create the complete infrastructure

terraform {
  required_version = ">= 1.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~>3.1"
    }
  }

  # Backend configuration - uncomment and configure for remote state
  # backend "azurerm" {
  #   resource_group_name  = "rg-terraform-state"
  #   storage_account_name = "sttfstatexxxxxx"
  #   container_name       = "tfstate"
  #   key                  = "todolist.terraform.tfstate"
  # }
}

# Configure the Azure Provider
provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location

  tags = var.tags
}

# Networking Module
module "networking" {
  source = "./modules/networking"

  project_name        = var.project_name
  environment         = var.environment
  location            = var.location
  resource_group_name = azurerm_resource_group.main.name
  vnet_address_space  = var.vnet_address_space
  subnet_configs      = var.subnet_configs
  private_dns_zones   = var.private_dns_zones
  tags                = var.tags
}

# Storage Module
module "storage" {
  source = "./modules/storage"

  project_name               = var.project_name
  environment                = var.environment
  location                   = var.location
  resource_group_name        = azurerm_resource_group.main.name
  storage_account_tier       = var.storage_account_tier
  storage_replication        = var.storage_replication
  enable_https_only          = var.enable_https_only
  min_tls_version            = "TLS1_2"
  enable_private_endpoints   = var.enable_private_endpoints
  app_subnet_id              = module.networking.app_subnet_id
  private_endpoint_subnet_id = module.networking.private_endpoint_subnet_id
  private_dns_zone_blob_id   = module.networking.private_dns_zone_ids["privatelink.blob.core.windows.net"]
  tags                       = var.tags
}

# App Service Module
module "app_service" {
  source = "./modules/app_service"

  app_name                   = var.app_name
  environment                = var.environment
  location                   = var.location
  resource_group_name        = azurerm_resource_group.main.name
  app_service_plan_sku       = var.app_service_plan_sku
  dotnet_version             = var.dotnet_version
  app_subnet_id              = module.networking.app_subnet_id
  private_endpoint_subnet_id = module.networking.private_endpoint_subnet_id
  private_dns_zone_webapp_id = module.networking.private_dns_zone_ids["privatelink.azurewebsites.net"]
  enable_private_endpoints   = var.enable_private_endpoints
  enable_https_only          = var.enable_https_only
  min_tls_version            = var.min_tls_version
  allowed_ips                = var.allowed_ips
  enable_app_insights        = var.enable_app_insights
  storage_connection_string  = var.enable_private_endpoints ? null : module.storage.storage_account_primary_connection_string

  additional_app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "1"
    "CORS_ALLOWED_ORIGINS"     = "*"
  }

  tags = var.tags

  depends_on = [
    module.networking,
    module.storage
  ]
}
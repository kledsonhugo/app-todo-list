# Storage Module for Todo List Application
# Creates storage account with private endpoint

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}

# Generate a random suffix for unique storage account name
resource "random_string" "storage_suffix" {
  length  = 8
  special = false
  upper   = false
}

# Storage Account
resource "azurerm_storage_account" "main" {
  name                     = "st${var.project_name}${var.environment}${random_string.storage_suffix.result}"
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = var.storage_account_tier
  account_replication_type = var.storage_replication

  # Security settings
  https_traffic_only_enabled      = var.enable_https_only
  min_tls_version                 = var.min_tls_version
  allow_nested_items_to_be_public = false
  shared_access_key_enabled       = true

  # Network rules - restrict access to VNet and private endpoints
  network_rules {
    default_action             = "Deny"
    virtual_network_subnet_ids = [var.app_subnet_id]
    bypass                     = ["AzureServices"]
  }

  tags = var.tags
}

# Blob Container for application files (if needed)
resource "azurerm_storage_container" "app_files" {
  name                  = "app-files"
  storage_account_name  = azurerm_storage_account.main.name
  container_access_type = "private"
}

# Private Endpoint for Storage Account (Blob service)
resource "azurerm_private_endpoint" "storage_blob_pe" {
  count = var.enable_private_endpoints ? 1 : 0

  name                = "pe-blob-${var.project_name}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_endpoint_subnet_id

  private_service_connection {
    name                           = "psc-blob-${var.project_name}-${var.environment}"
    private_connection_resource_id = azurerm_storage_account.main.id
    subresource_names              = ["blob"]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [var.private_dns_zone_blob_id]
  }

  tags = var.tags
}
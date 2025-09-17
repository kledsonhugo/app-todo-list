# Outputs for Storage Module

output "storage_account_id" {
  description = "Storage account ID"
  value       = azurerm_storage_account.main.id
}

output "storage_account_name" {
  description = "Storage account name"
  value       = azurerm_storage_account.main.name
}

output "storage_account_primary_connection_string" {
  description = "Storage account primary connection string"
  value       = azurerm_storage_account.main.primary_connection_string
  sensitive   = true
}

output "storage_account_primary_access_key" {
  description = "Storage account primary access key"
  value       = azurerm_storage_account.main.primary_access_key
  sensitive   = true
}

output "blob_container_name" {
  description = "Blob container name"
  value       = azurerm_storage_container.app_files.name
}

output "private_endpoint_id" {
  description = "Storage account private endpoint ID"
  value       = var.enable_private_endpoints ? azurerm_private_endpoint.storage_blob_pe[0].id : null
}
# Output values for the Todo List Application Infrastructure

output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "resource_group_location" {
  description = "Location of the resource group"
  value       = azurerm_resource_group.main.location
}

# Networking Outputs
output "vnet_id" {
  description = "Virtual network ID"
  value       = module.networking.vnet_id
}

output "vnet_name" {
  description = "Virtual network name"
  value       = module.networking.vnet_name
}

output "app_subnet_id" {
  description = "Application subnet ID"
  value       = module.networking.app_subnet_id
}

output "private_endpoint_subnet_id" {
  description = "Private endpoint subnet ID"
  value       = module.networking.private_endpoint_subnet_id
}

# Storage Outputs
output "storage_account_name" {
  description = "Storage account name"
  value       = module.storage.storage_account_name
}

output "storage_account_id" {
  description = "Storage account ID"
  value       = module.storage.storage_account_id
}

# App Service Outputs
output "app_service_name" {
  description = "App Service name"
  value       = module.app_service.app_service_name
}

output "app_service_url" {
  description = "App Service URL"
  value       = module.app_service.app_service_url
}

output "app_service_default_hostname" {
  description = "App Service default hostname"
  value       = module.app_service.app_service_default_hostname
}

# Application Insights Outputs
output "application_insights_id" {
  description = "Application Insights ID"
  value       = module.app_service.application_insights_id
}

output "application_insights_instrumentation_key" {
  description = "Application Insights instrumentation key"
  value       = module.app_service.application_insights_instrumentation_key
  sensitive   = true
}

# Security Information
output "app_service_principal_id" {
  description = "App Service managed identity principal ID"
  value       = module.app_service.app_service_principal_id
}

# Deployment Information
output "deployment_instructions" {
  description = "Instructions for deploying the Todo List application"
  value       = <<EOT
Todo List Application Infrastructure Deployed Successfully!

Next Steps:
1. Deploy your .NET application to: ${module.app_service.app_service_url}
2. Configure your CI/CD pipeline to deploy to the App Service
3. Update your application settings if needed
4. Test your application at the provided URL

App Service Details:
- Name: ${module.app_service.app_service_name}
- URL: ${module.app_service.app_service_url}
- Resource Group: ${azurerm_resource_group.main.name}

Security Features:
- Private endpoints enabled: ${var.enable_private_endpoints}
- HTTPS only: ${var.enable_https_only}
- VNet integration: Enabled
- Application Insights: ${var.enable_app_insights ? "Enabled" : "Disabled"}
EOT
}
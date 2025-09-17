# Variables for App Service Module

variable "app_name" {
  description = "Name of the application"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "app_service_plan_sku" {
  description = "SKU for the App Service Plan"
  type        = string
  default     = "B1"
}

variable "dotnet_version" {
  description = ".NET version for the application"
  type        = string
  default     = "8.0"
}

variable "app_subnet_id" {
  description = "App subnet ID for VNet integration"
  type        = string
}

variable "private_endpoint_subnet_id" {
  description = "Private endpoint subnet ID"
  type        = string
}

variable "private_dns_zone_webapp_id" {
  description = "Private DNS zone ID for web apps"
  type        = string
}

variable "enable_private_endpoints" {
  description = "Enable private endpoints for App Service"
  type        = bool
  default     = true
}

variable "enable_https_only" {
  description = "Enable HTTPS only"
  type        = bool
  default     = true
}

variable "min_tls_version" {
  description = "Minimum TLS version"
  type        = string
  default     = "1.2"
}

variable "allowed_ips" {
  description = "List of allowed IP addresses"
  type        = list(string)
  default     = []
}

variable "enable_app_insights" {
  description = "Enable Application Insights"
  type        = bool
  default     = true
}

variable "storage_connection_string" {
  description = "Storage connection string"
  type        = string
  default     = null
  sensitive   = true
}

variable "additional_app_settings" {
  description = "Additional application settings"
  type        = map(string)
  default     = {}
}

variable "tags" {
  description = "Common tags for all resources"
  type        = map(string)
  default     = {}
}
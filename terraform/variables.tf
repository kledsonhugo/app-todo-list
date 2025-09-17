# Variables for Todo List Application Infrastructure

variable "environment" {
  description = "Environment name (e.g., dev, staging, production)"
  type        = string
  validation {
    condition     = can(regex("^(dev|staging|production)$", var.environment))
    error_message = "Environment must be one of: dev, staging, production."
  }
}

variable "project_name" {
  description = "Name of the project"
  type        = string
}

variable "location" {
  description = "Azure region for resource deployment"
  type        = string
  default     = "East US"
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "app_name" {
  description = "Name of the application"
  type        = string
}

variable "app_service_plan_sku" {
  description = "SKU for the App Service Plan"
  type        = string
  default     = "B1"
  validation {
    condition     = can(regex("^(F1|D1|B1|B2|B3|S1|S2|S3|P1|P2|P3|P1V2|P2V2|P3V2|P1V3|P2V3|P3V3)$", var.app_service_plan_sku))
    error_message = "App Service Plan SKU must be a valid Azure App Service Plan SKU."
  }
}

variable "dotnet_version" {
  description = ".NET version for the application"
  type        = string
  default     = "8.0"
}

variable "vnet_address_space" {
  description = "Address space for the virtual network"
  type        = list(string)
  default     = ["10.0.0.0/16"]
}

variable "subnet_configs" {
  description = "Configuration for subnets"
  type = map(object({
    address_prefixes  = list(string)
    service_endpoints = list(string)
  }))
}

variable "allowed_ips" {
  description = "List of allowed IP addresses for access restrictions"
  type        = list(string)
  default     = []
}

variable "storage_account_tier" {
  description = "Storage account performance tier"
  type        = string
  default     = "Standard"
  validation {
    condition     = can(regex("^(Standard|Premium)$", var.storage_account_tier))
    error_message = "Storage account tier must be either Standard or Premium."
  }
}

variable "storage_replication" {
  description = "Storage account replication type"
  type        = string
  default     = "LRS"
  validation {
    condition     = can(regex("^(LRS|GRS|RAGRS|ZRS|GZRS|RAGZRS)$", var.storage_replication))
    error_message = "Storage replication must be a valid Azure storage replication type."
  }
}

variable "enable_private_endpoints" {
  description = "Enable private endpoints for services"
  type        = bool
  default     = true
}

variable "private_dns_zones" {
  description = "List of private DNS zones to create"
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "Common tags for all resources"
  type        = map(string)
  default     = {}
}

variable "enable_https_only" {
  description = "Enable HTTPS only for web services"
  type        = bool
  default     = true
}

variable "min_tls_version" {
  description = "Minimum TLS version"
  type        = string
  default     = "1.2"
}

variable "enable_app_insights" {
  description = "Enable Application Insights"
  type        = bool
  default     = true
}
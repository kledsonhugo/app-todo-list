# Variables for Storage Module

variable "project_name" {
  description = "Name of the project"
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

variable "storage_account_tier" {
  description = "Storage account performance tier"
  type        = string
  default     = "Standard"
}

variable "storage_replication" {
  description = "Storage account replication type"
  type        = string
  default     = "LRS"
}

variable "enable_https_only" {
  description = "Enable HTTPS only for storage account"
  type        = bool
  default     = true
}

variable "min_tls_version" {
  description = "Minimum TLS version"
  type        = string
  default     = "TLS1_2"
}

variable "enable_private_endpoints" {
  description = "Enable private endpoints for storage"
  type        = bool
  default     = true
}

variable "app_subnet_id" {
  description = "App subnet ID for network rules"
  type        = string
}

variable "private_endpoint_subnet_id" {
  description = "Private endpoint subnet ID"
  type        = string
}

variable "private_dns_zone_blob_id" {
  description = "Private DNS zone ID for blob storage"
  type        = string
}

variable "tags" {
  description = "Common tags for all resources"
  type        = map(string)
  default     = {}
}
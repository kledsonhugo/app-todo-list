# Outputs for Networking Module

output "vnet_id" {
  description = "Virtual network ID"
  value       = azurerm_virtual_network.main.id
}

output "vnet_name" {
  description = "Virtual network name"
  value       = azurerm_virtual_network.main.name
}

output "subnet_ids" {
  description = "Map of subnet names to their IDs"
  value = {
    for name, subnet in azurerm_subnet.subnets : name => subnet.id
  }
}

output "app_subnet_id" {
  description = "App subnet ID"
  value       = azurerm_subnet.subnets["app_subnet"].id
}

output "private_endpoint_subnet_id" {
  description = "Private endpoint subnet ID"
  value       = azurerm_subnet.subnets["private_endpoint_subnet"].id
}

output "private_dns_zone_ids" {
  description = "Map of private DNS zone names to their IDs"
  value = {
    for name, zone in azurerm_private_dns_zone.private_dns_zones : name => zone.id
  }
}

output "app_nsg_id" {
  description = "App subnet NSG ID"
  value       = azurerm_network_security_group.app_nsg.id
}

output "pe_nsg_id" {
  description = "Private endpoint subnet NSG ID"
  value       = azurerm_network_security_group.pe_nsg.id
}
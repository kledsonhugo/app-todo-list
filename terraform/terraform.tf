# Terraform configuration file for backend storage

terraform {
  # Uncomment and configure for remote state storage
  # backend "azurerm" {
  #   resource_group_name  = "rg-terraform-state"
  #   storage_account_name = "sttfstatexxxxxx"
  #   container_name       = "tfstate"
  #   key                  = "todolist/terraform.tfstate"
  # }
}
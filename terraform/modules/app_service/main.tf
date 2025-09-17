# App Service Module for Todo List Application
# Creates App Service Plan, Web App, and Application Insights

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}

# Application Insights
resource "azurerm_application_insights" "main" {
  count = var.enable_app_insights ? 1 : 0

  name                = "ai-${var.app_name}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = "web"

  tags = var.tags
}

# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "asp-${var.app_name}-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = "Linux"
  sku_name            = var.app_service_plan_sku

  tags = var.tags
}

# Web App
resource "azurerm_linux_web_app" "main" {
  name                = "app-${var.app_name}-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = azurerm_service_plan.main.id

  # VNet Integration
  virtual_network_subnet_id = var.app_subnet_id

  site_config {
    always_on               = var.app_service_plan_sku != "F1" && var.app_service_plan_sku != "D1"
    ftps_state              = "Disabled"
    http2_enabled           = true
    minimum_tls_version     = var.min_tls_version
    scm_minimum_tls_version = var.min_tls_version
    use_32_bit_worker       = false
    vnet_route_all_enabled  = true

    application_stack {
      dotnet_version = var.dotnet_version
    }

    # IP restrictions for enhanced security
    dynamic "ip_restriction" {
      for_each = var.allowed_ips
      content {
        ip_address = ip_restriction.value
        action     = "Allow"
        priority   = 1000 + ip_restriction.key
        name       = "AllowedIP_${ip_restriction.key}"
      }
    }
  }

  # Application settings
  app_settings = merge(
    {
      "ASPNETCORE_ENVIRONMENT"              = var.environment == "production" ? "Production" : "Development"
      "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
      "SCM_DO_BUILD_DURING_DEPLOYMENT"      = "true"
    },
    var.enable_app_insights ? {
      "APPINSIGHTS_INSTRUMENTATIONKEY"             = azurerm_application_insights.main[0].instrumentation_key
      "APPLICATIONINSIGHTS_CONNECTION_STRING"      = azurerm_application_insights.main[0].connection_string
      "ApplicationInsightsAgent_EXTENSION_VERSION" = "~3"
    } : {},
    var.additional_app_settings
  )

  # Connection strings would be added here if needed
  # dynamic "connection_string" {
  #   for_each = var.storage_connection_string != null ? [1] : []
  #   content {
  #     name  = "DefaultConnection"
  #     type  = "Custom"
  #     value = var.storage_connection_string
  #   }
  # }

  # Enable HTTPS only
  https_only = var.enable_https_only

  # Identity for managed service identity
  identity {
    type = "SystemAssigned"
  }

  tags = var.tags

  lifecycle {
    ignore_changes = [
      # Ignore changes to these settings as they may be managed by deployment pipelines
      site_config[0].application_stack[0].dotnet_version,
    ]
  }
}

# Private Endpoint for Web App
resource "azurerm_private_endpoint" "webapp_pe" {
  count = var.enable_private_endpoints ? 1 : 0

  name                = "pe-webapp-${var.app_name}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_endpoint_subnet_id

  private_service_connection {
    name                           = "psc-webapp-${var.app_name}-${var.environment}"
    private_connection_resource_id = azurerm_linux_web_app.main.id
    subresource_names              = ["sites"]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [var.private_dns_zone_webapp_id]
  }

  tags = var.tags
}

# Custom domain and SSL certificate (optional)
# This would be configured based on specific domain requirements
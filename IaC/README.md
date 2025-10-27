# Infrastructure as Code (IaC) - Todo List Application

This folder contains Azure Bicep templates and deployment scripts for the Todo List Application infrastructure.

## 🏗️ Architecture Overview

The infrastructure implements a secure, scalable Azure architecture with the following components:

- **App Service** with Easy Auth for .NET 8.0 Todo List API
- **Azure SQL Database** for data persistence
- **Managed Identity** for secure authentication
- **Application Insights** for monitoring and observability
- **Microsoft Entra ID** for user authentication

All resources use the **TDC-** prefix as requested in the requirements.

## 📁 Folder Structure

```
IaC/
├── main.bicep                    # Main monolithic Bicep template
├── main-modular.bicep           # Main template using modules
├── modules/                     # Reusable Bicep modules
│   ├── appService.bicep         # App Service with Easy Auth
│   ├── sqlDatabase.bicep        # SQL Server and Database  
│   ├── monitoring.bicep         # Application Insights & Log Analytics
│   ├── identity.bicep           # Managed Identity
│   └── roleAssignments.bicep    # RBAC role assignments
├── parameters/                  # Environment-specific parameters
│   ├── dev.parameters.json      # Development environment
│   └── prod.parameters.json     # Production environment
├── deploy.ps1                   # PowerShell deployment script
├── deploy.sh                    # Bash deployment script
├── architecture-diagram.md      # Mermaid architecture diagram
└── README.md                    # This file
```

## 🚀 Quick Start

### Prerequisites

1. **Azure CLI** installed and configured
2. **Azure subscription** with appropriate permissions
3. **PowerShell** (for Windows) or **Bash** (for Linux/macOS)

### Deployment Options

#### Option 1: Using PowerShell (Windows/Cross-platform)

```powershell
# Basic deployment to development environment
.\deploy.ps1 -SubscriptionId "your-subscription-id" -ResourceGroupName "TDC-dev-rg"

# Production deployment with custom location
.\deploy.ps1 -SubscriptionId "your-subscription-id" -ResourceGroupName "TDC-prod-rg" -Environment "prod" -Location "West US 2"
```

#### Option 2: Using Bash (Linux/macOS/WSL)

```bash
# Make script executable (first time only)
chmod +x deploy.sh

# Basic deployment to development environment
./deploy.sh -s "your-subscription-id" -g "TDC-dev-rg"

# Production deployment with modular template
./deploy.sh -s "your-subscription-id" -g "TDC-prod-rg" -e prod -l "West US 2" -m
```

#### Option 3: Direct Azure CLI

```bash
# Login to Azure
az login

# Set subscription
az account set --subscription "your-subscription-id"

# Create resource group
az group create --name "TDC-dev-rg" --location "East US"

# Deploy using monolithic template
az deployment group create \
  --resource-group "TDC-dev-rg" \
  --template-file main.bicep \
  --parameters location="East US" environmentName="dev" sqlAdminLogin="tdcadmin" sqlAdminPassword="YourSecurePassword123!"

# Or deploy using modular template
az deployment group create \
  --resource-group "TDC-dev-rg" \
  --template-file main-modular.bicep \
  --parameters location="East US" environmentName="dev" sqlAdminLogin="tdcadmin" sqlAdminPassword="YourSecurePassword123!"
```

## 🔧 Configuration

### Parameters

| Parameter | Description | Required | Default |
|-----------|-------------|----------|---------|
| `location` | Azure region for resources | No | East US |
| `environmentName` | Environment (dev/test/prod) | No | dev |
| `appNamePrefix` | Prefix for resource names | No | TDC |
| `sqlAdminLogin` | SQL Server admin username | Yes | - |
| `sqlAdminPassword` | SQL Server admin password | Yes | - |
| `customDomain` | Custom domain (optional) | No | - |

### Environment-Specific Configuration

Edit the parameter files in the `parameters/` folder to customize settings for each environment:

- `dev.parameters.json` - Development environment settings
- `prod.parameters.json` - Production environment settings

### Security Best Practices

1. **Password Management**: Store SQL admin passwords in Azure Key Vault
2. **Parameter Files**: Use Key Vault references in parameter files (see examples)
3. **Access Control**: Implement least privilege access with RBAC
4. **Network Security**: Consider using Private Endpoints for production

## 📊 Deployed Resources

After successful deployment, the following resources will be created:

### Development Environment (TDC-dev-*)
- `TDC-dev-rg` - Resource Group
- `TDC-dev-app` - App Service (Todo List API)
- `TDC-dev-asp` - App Service Plan
- `TDC-dev-sqlserver` - SQL Server
- `TDC-dev-sqldb` - SQL Database
- `TDC-dev-mi` - Managed Identity
- `TDC-dev-ai` - Application Insights
- `TDC-dev-law` - Log Analytics Workspace

### Production Environment (TDC-prod-*)
Same naming pattern with "prod" environment suffix.

## 🔍 Monitoring & Troubleshooting

### Application Insights
- Application performance monitoring
- Real-time metrics and alerts
- Distributed tracing
- Custom telemetry

### Log Analytics
- Centralized logging
- Query and analysis capabilities
- Custom dashboards
- Integration with Azure Monitor

### Common Issues

1. **Template Validation Errors**: Check Bicep syntax and parameter values
2. **Permission Issues**: Ensure proper RBAC permissions for deployment
3. **Resource Naming Conflicts**: Use unique resource group names
4. **SQL Password Requirements**: Ensure password meets complexity requirements

## 🛡️ Security Features

### Authentication & Authorization
- **Easy Auth**: Built-in App Service authentication
- **Microsoft Entra ID**: Enterprise identity provider
- **Managed Identity**: Password-less authentication to Azure services

### Network Security
- **HTTPS Only**: All communication encrypted
- **TLS 1.2+**: Modern encryption standards
- **Private Networking**: Optional VNet integration

### Data Protection
- **SQL Database**: Encrypted at rest and in transit
- **Connection Strings**: Secured using Managed Identity
- **Key Management**: Integration with Azure Key Vault

## 📈 Scaling & Performance

### App Service Scaling
- **Vertical Scaling**: Upgrade to higher SKUs for more CPU/RAM
- **Horizontal Scaling**: Auto-scale rules based on metrics
- **Staging Slots**: Blue-green deployments

### Database Performance
- **DTU/vCore**: Scale database compute resources
- **Storage**: Independent storage scaling
- **Read Replicas**: Offload read workloads

## 🔄 CI/CD Integration

### Azure DevOps
```yaml
# Example pipeline step
- task: AzureResourceManagerTemplateDeployment@3
  displayName: 'Deploy Infrastructure'
  inputs:
    deploymentScope: 'Resource Group'
    azureResourceManagerConnection: 'your-service-connection'
    subscriptionId: 'your-subscription-id'
    action: 'Create Or Update Resource Group'
    resourceGroupName: '$(resourceGroupName)'
    location: '$(location)'
    templateLocation: 'Linked artifact'
    csmFile: 'IaC/main.bicep'
    csmParametersFile: 'IaC/parameters/$(environment).parameters.json'
```

### GitHub Actions
```yaml
# Example workflow step
- name: Deploy to Azure
  uses: azure/arm-deploy@v1
  with:
    subscriptionId: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
    resourceGroupName: ${{ env.RESOURCE_GROUP_NAME }}
    template: ./IaC/main.bicep
    parameters: ./IaC/parameters/${{ env.ENVIRONMENT }}.parameters.json
```

## 🏷️ Resource Tags

All resources are automatically tagged with:
- `Environment`: dev/test/prod
- `Application`: TodoList
- `ManagedBy`: Bicep
- `DeploymentDate`: Deployment timestamp

## 🆘 Support & Troubleshooting

### Deployment Issues
1. Check Azure CLI version and login status
2. Verify subscription permissions
3. Review Bicep template validation output
4. Check resource availability in target region

### Runtime Issues
1. Check Application Insights for application errors
2. Review App Service logs and metrics  
3. Verify database connectivity and performance
4. Check Managed Identity role assignments

### Getting Help
- Review Azure documentation for specific services
- Use Azure Support for critical production issues
- Check GitHub Issues for template-related problems

---

## 🎯 Next Steps

1. **Deploy Application Code**: Use the deployed App Service for your .NET application
2. **Configure Custom Domain**: Set up custom domain and SSL certificates
3. **Set up Monitoring**: Configure alerts and dashboards in Application Insights
4. **Implement CI/CD**: Automate deployments using Azure DevOps or GitHub Actions
5. **Security Review**: Implement additional security measures for production

For the complete architecture diagram, see [architecture-diagram.md](./architecture-diagram.md).
@description('Location for all resources.')
param location string = resourceGroup().location

@description('Name of the App Service Plan')
param appServicePlanName string

@description('Name of the App Service')
param appServiceName string

@description('Environment name')
param environmentName string

@description('Application Insights connection string')
param applicationInsightsConnectionString string

@description('SQL Server FQDN')
param sqlServerFqdn string

@description('SQL Database name')
param sqlDatabaseName string

@description('Managed Identity resource ID')
param managedIdentityId string

@description('Managed Identity client ID')
param managedIdentityClientId string

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  properties: {
    reserved: false
  }
}

// App Service
resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: appServiceName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      metadata: [
        {
          name: 'CURRENT_STACK'
          value: 'dotnet'
        }
      ]
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'Recommended'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environmentName == 'prod' ? 'Production' : 'Development'
        }
      ]
      connectionStrings: [
        {
          name: 'DefaultConnection'
          connectionString: 'Server=tcp:${sqlServerFqdn},1433;Initial Catalog=${sqlDatabaseName};Authentication=Active Directory Managed Identity;User Id=${managedIdentityClientId};'
          type: 'SQLAzure'
        }
      ]
      alwaysOn: true
      http20Enabled: true
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
    clientAffinityEnabled: false
  }
}

// Authentication/Authorization configuration (Easy Auth)
resource authSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: appService
  name: 'authsettingsV2'
  properties: {
    globalValidation: {
      requireAuthentication: true
      unauthenticatedClientAction: 'RedirectToLoginPage'
    }
    identityProviders: {
      azureActiveDirectory: {
        enabled: true
        registration: {
          openIdIssuer: 'https://sts.windows.net/${tenant().tenantId}/'
          clientId: managedIdentityClientId
        }
        validation: {
          allowedAudiences: [
            'api://${managedIdentityClientId}'
          ]
        }
      }
    }
    login: {
      routes: {
        logoutEndpoint: '/.auth/logout'
      }
    }
    httpSettings: {
      requireHttps: true
      routes: {
        apiPrefix: '/.auth'
      }
    }
  }
}

// Outputs
output appServiceName string = appService.name
output appServiceUrl string = 'https://${appService.properties.defaultHostName}'
output appServicePlanId string = appServicePlan.id
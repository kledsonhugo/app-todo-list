@description('Location for all resources.')
param location string = resourceGroup().location

@description('Environment name (e.g., dev, test, prod)')
param environmentName string = 'dev'

@description('Application name prefix')
param appNamePrefix string = 'TDC'

@description('SQL Server administrator login')
@secure()
param sqlAdminLogin string

@description('SQL Server administrator password')
@secure()
param sqlAdminPassword string

// Variables
var resourceNamePrefix = '${appNamePrefix}-${environmentName}'
var appServicePlanName = '${resourceNamePrefix}-asp'
var appServiceName = '${resourceNamePrefix}-app'
var sqlServerName = '${resourceNamePrefix}-sqlserver'
var sqlDatabaseName = '${resourceNamePrefix}-sqldb'
var applicationInsightsName = '${resourceNamePrefix}-ai'
var logAnalyticsWorkspaceName = '${resourceNamePrefix}-law'
var managedIdentityName = '${resourceNamePrefix}-mi'

// Deploy Monitoring (Application Insights + Log Analytics)
module monitoring 'modules/monitoring.bicep' = {
  name: 'monitoring-deployment'
  params: {
    location: location
    applicationInsightsName: applicationInsightsName
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

// Deploy Managed Identity
module identity 'modules/identity.bicep' = {
  name: 'identity-deployment'
  params: {
    location: location
    managedIdentityName: managedIdentityName
  }
}

// Deploy SQL Database
module sqlDatabase 'modules/sqlDatabase.bicep' = {
  name: 'sql-deployment'
  params: {
    location: location
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    sqlAdminLogin: sqlAdminLogin
    sqlAdminPassword: sqlAdminPassword
    managedIdentityId: identity.outputs.managedIdentityId
  }
}

// Reference to existing SQL Database for role assignment
resource existingSqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' existing = {
  name: '${sqlServerName}/${sqlDatabaseName}'
  dependsOn: [
    sqlDatabase
  ]
}

// Deploy role assignments using existing resource
resource sqlDbContributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: existingSqlDatabase
  name: guid(resourceGroup().id, managedIdentityName, sqlDatabaseName, 'SQL DB Contributor')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec') // SQL DB Contributor
    principalId: identity.outputs.managedIdentityPrincipalId
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    sqlDatabase
  ]
}

// Deploy App Service
module appService 'modules/appService.bicep' = {
  name: 'appservice-deployment'
  params: {
    location: location
    appServicePlanName: appServicePlanName
    appServiceName: appServiceName
    environmentName: environmentName
    applicationInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    sqlServerFqdn: sqlDatabase.outputs.sqlServerFqdn
    sqlDatabaseName: sqlDatabaseName
    managedIdentityId: identity.outputs.managedIdentityId
    managedIdentityClientId: identity.outputs.managedIdentityClientId
  }
  dependsOn: [
    sqlDbContributorRoleAssignment
  ]
}

// Outputs
output appServiceName string = appService.outputs.appServiceName
output appServiceUrl string = appService.outputs.appServiceUrl
output sqlServerName string = sqlDatabase.outputs.sqlServerName
output sqlDatabaseName string = sqlDatabase.outputs.sqlDatabaseName
output applicationInsightsName string = monitoring.outputs.applicationInsightsName
output managedIdentityName string = identity.outputs.managedIdentityName
output managedIdentityClientId string = identity.outputs.managedIdentityClientId
output resourceGroupName string = resourceGroup().name